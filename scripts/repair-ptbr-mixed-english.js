const fs = require('fs');

const enPath = 'services/workflow/src/ResourceryPlatformWorkflow.Workflow.Domain.Shared/Localization/Workflow/en.json';
const ptPath = 'services/workflow/src/ResourceryPlatformWorkflow.Workflow.Domain.Shared/Localization/Workflow/pt-BR.json';

const en = JSON.parse(fs.readFileSync(enPath, 'utf8'));
const pt = JSON.parse(fs.readFileSync(ptPath, 'utf8'));

const englishMarkers = /\b(the|with|from|request|office|director|staff|meeting|draft|approved|submit|review|once|upon|while|through|should|will|can|delivered|dispatch|output|feedback|copy|hard|soft|check|follow-up|supporting|documents|team|service|payment|invoice|travel|processing|instructions)\b/i;

const keysToRepair = Object.keys(pt.texts).filter((k) => {
  const current = pt.texts[k];
  const source = en.texts[k];
  if (typeof current !== 'string' || typeof source !== 'string') {
    return false;
  }
  if (current === source) {
    return true;
  }
  return englishMarkers.test(current);
});

async function translateToPtBr(text) {
  const url = `https://api.mymemory.translated.net/get?q=${encodeURIComponent(text)}&langpair=en|pt-BR`;
  const response = await fetch(url);
  if (!response.ok) {
    throw new Error(`HTTP ${response.status}`);
  }
  const data = await response.json();
  const translated = data && data.responseData && data.responseData.translatedText;
  return translated || text;
}

function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

(async () => {
  console.log(`Suspect keys to repair: ${keysToRepair.length}`);
  let repaired = 0;
  let failed = 0;

  for (let i = 0; i < keysToRepair.length; i++) {
    const key = keysToRepair[i];
    const source = en.texts[key];

    try {
      const translated = await translateToPtBr(source);
      if (translated && translated !== pt.texts[key]) {
        pt.texts[key] = translated;
        repaired++;
      }
    } catch (error) {
      failed++;
      console.error(`Failed ${i + 1}/${keysToRepair.length}: ${key} -> ${error.message}`);
    }

    if ((i + 1) % 25 === 0) {
      fs.writeFileSync(ptPath, JSON.stringify(pt, null, 2) + '\n', 'utf8');
      console.log(`Progress ${i + 1}/${keysToRepair.length}, repaired: ${repaired}, failed: ${failed}`);
    }

    await sleep(250);
  }

  fs.writeFileSync(ptPath, JSON.stringify(pt, null, 2) + '\n', 'utf8');
  console.log(`Done. Repaired: ${repaired}, Failed: ${failed}`);
})();
