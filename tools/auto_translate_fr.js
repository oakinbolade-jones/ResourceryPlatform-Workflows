const fs = require('fs');
const path = require('path');
const fetch = global.fetch || require('node-fetch');

const enPath = path.resolve(__dirname, '..', 'services', 'workflow', 'src', 'ResourceryPlatformWorkflow.Workflow.Domain.Shared', 'Localization', 'Workflow', 'en.json');
const frPath = path.resolve(__dirname, '..', 'services', 'workflow', 'src', 'ResourceryPlatformWorkflow.Workflow.Domain.Shared', 'Localization', 'Workflow', 'fr.json');
const reportPath = path.resolve(__dirname, 'auto_translate_fr_report.json');

function safeRead(file) {
  try {
    return fs.readFileSync(file, 'utf8');
  } catch (e) {
    console.error('READ_ERROR', file, e.message);
    process.exit(1);
  }
}

const stripBOM = s => (s && s.charCodeAt(0) === 0xfeff ? s.slice(1) : s);
const en = JSON.parse(stripBOM(safeRead(enPath)));
const fr = JSON.parse(stripBOM(safeRead(frPath)));

const enTexts = en.texts || {};
const frTexts = fr.texts || {};
const report = { translated: [], failed: [] };

const keysToTranslate = Object.keys(enTexts).filter(k => frTexts[k] === enTexts[k]);
console.log('Keys to translate:', keysToTranslate.length);

const ENDPOINTS = [
  'https://libretranslate.com/translate',
  'https://translate.astian.org/translate',
  'https://libretranslate.de/translate'
];

async function translateText(text) {
  let lastErr = null;

  for (const url of ENDPOINTS) {
    try {
      const res = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'User-Agent': 'auto-translate-fr-script/1.0'
        },
        body: JSON.stringify({ q: text, source: 'en', target: 'fr', format: 'text' })
      });

      if (!res.ok) {
        throw new Error('HTTP ' + res.status + ' from ' + url);
      }

      const body = await res.text();
      try {
        const j = JSON.parse(body);
        if (j && j.translatedText) {
          return j.translatedText;
        }
        throw new Error('No translatedText in response from ' + url);
      } catch (e) {
        lastErr = new Error('Invalid JSON from ' + url + ': ' + e.message);
      }
    } catch (e) {
      lastErr = e;
    }
  }

  // Last-resort fallback
  try {
    const googleUrl = 'https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=fr&dt=t&q=' + encodeURIComponent(text);
    const res = await fetch(googleUrl, {
      method: 'GET',
      headers: { 'User-Agent': 'auto-translate-fr-script/1.0' }
    });

    if (res.ok) {
      const j = await res.json();
      if (Array.isArray(j) && j[0] && j[0][0] && j[0][0][0]) {
        return j[0][0][0];
      }
      throw new Error('Unexpected response shape from Google fallback');
    }

    throw new Error('Google fallback HTTP ' + res.status);
  } catch (e) {
    lastErr = e;
  }

  throw lastErr || new Error('No translation endpoint succeeded');
}

async function main() {
  const limit = process.env.TRANSLATE_LIMIT ? parseInt(process.env.TRANSLATE_LIMIT, 10) : undefined;
  const keys = limit ? keysToTranslate.slice(0, limit) : keysToTranslate;

  console.log('Translating', keys.length, 'keys (limit:', limit || 'none', ')');

  for (let i = 0; i < keys.length; i++) {
    const key = keys[i];
    const text = enTexts[key];

    try {
      const translated = await translateText(text);
      frTexts[key] = translated;
      report.translated.push({ key, translated });
    } catch (e) {
      console.error('TRANSLATE_ERROR', key, e.message);
      report.failed.push({ key, error: String(e.message || e) });
    }

    await new Promise(r => setTimeout(r, 200));
  }

  const out = { culture: 'fr', texts: frTexts };
  fs.writeFileSync(frPath, JSON.stringify(out, null, 2) + '\n', 'utf8');
  fs.writeFileSync(reportPath, JSON.stringify(report, null, 2), 'utf8');

  console.log('DONE. translated:', report.translated.length, 'failed:', report.failed.length);
}

main().catch(e => {
  console.error(e);
  process.exit(1);
});
