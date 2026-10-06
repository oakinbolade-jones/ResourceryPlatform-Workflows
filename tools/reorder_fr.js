const fs = require('fs');
const path = require('path');

const enPath = path.resolve(__dirname, '..', 'services', 'workflow', 'src', 'ResourceryPlatformWorkflow.Workflow.Domain.Shared', 'Localization', 'Workflow', 'en.json');
const frPath = path.resolve(__dirname, '..', 'services', 'workflow', 'src', 'ResourceryPlatformWorkflow.Workflow.Domain.Shared', 'Localization', 'Workflow', 'fr.json');
const reportPath = path.resolve(__dirname, 'reorder_fr_report.json');

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

const outTexts = {};
const missingKeys = [];

for (const key of Object.keys(enTexts)) {
  if (Object.prototype.hasOwnProperty.call(frTexts, key)) {
    outTexts[key] = frTexts[key];
  } else {
    outTexts[key] = enTexts[key];
    missingKeys.push(key);
  }
}

const extraKeys = Object.keys(frTexts).filter(k => !Object.prototype.hasOwnProperty.call(enTexts, k));

const out = { culture: 'fr', texts: outTexts };
fs.writeFileSync(frPath, JSON.stringify(out, null, 2) + '\n', 'utf8');

const report = {
  missingCount: missingKeys.length,
  extraCount: extraKeys.length,
  missingKeys,
  extraKeys
};
fs.writeFileSync(reportPath, JSON.stringify(report, null, 2), 'utf8');

console.log('WROTE', frPath);
console.log('REPORT', JSON.stringify({ missingCount: report.missingCount, extraCount: report.extraCount }));
