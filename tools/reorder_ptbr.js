const fs = require('fs');
const path = require('path');

const enPath = path.resolve(__dirname, '..', 'services', 'workflow', 'src', 'ResourceryPlatformWorkflow.Workflow.Domain.Shared', 'Localization', 'Workflow', 'en.json');
const ptPath = path.resolve(__dirname, '..', 'services', 'workflow', 'src', 'ResourceryPlatformWorkflow.Workflow.Domain.Shared', 'Localization', 'Workflow', 'pt-BR.json');

function safeRead(file) {
  try { return fs.readFileSync(file, 'utf8'); } catch (e) { console.error('READ_ERROR', file, e.message); process.exit(1); }
}

const stripBOM = s => s && s.charCodeAt(0) === 0xfeff ? s.slice(1) : s;
const en = JSON.parse(stripBOM(safeRead(enPath)));
const pt = JSON.parse(stripBOM(safeRead(ptPath)));

const enTexts = en.texts || {};
const ptTexts = pt.texts || {};

const newTexts = {};
const missingKeys = [];
for (const key of Object.keys(enTexts)) {
  if (Object.prototype.hasOwnProperty.call(ptTexts, key)) {
    newTexts[key] = ptTexts[key];
  } else {
    newTexts[key] = enTexts[key];
    missingKeys.push(key);
  }
}

// find extra keys present in pt but not in en
const extraKeys = Object.keys(ptTexts).filter(k => !Object.prototype.hasOwnProperty.call(enTexts, k));

const out = { culture: 'pt-BR', texts: newTexts };
fs.writeFileSync(ptPath, JSON.stringify(out, null, 2) + '\n', 'utf8');

const report = { missingKeys, extraKeys, missingCount: missingKeys.length, extraCount: extraKeys.length };
fs.writeFileSync(path.resolve(__dirname, 'reorder_ptbr_report.json'), JSON.stringify(report, null, 2), 'utf8');

console.log('WROTE', ptPath);
console.log('REPORT', JSON.stringify(report));
