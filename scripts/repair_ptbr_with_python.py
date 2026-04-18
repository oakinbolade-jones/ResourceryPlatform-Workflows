import json
import re
import time
from pathlib import Path
from urllib.parse import quote
from urllib.request import urlopen

ROOT = Path('services/workflow/src/ResourceryPlatformWorkflow.Workflow.Domain.Shared/Localization/Workflow')
EN_PATH = ROOT / 'en.json'
PT_PATH = ROOT / 'pt-BR.json'

with EN_PATH.open('r', encoding='utf-8') as f:
    en_data = json.load(f)

with PT_PATH.open('r', encoding='utf-8') as f:
    pt_data = json.load(f)

texts_en = en_data.get('texts', {})
texts_pt = pt_data.get('texts', {})

english_markers = re.compile(
    r"\b(the|with|from|request|office|director|staff|meeting|draft|approved|submit|review|once|upon|while|through|should|will|can|delivered|dispatch|output|feedback|copy|hard|soft|check|follow-up|supporting|documents|team|service|payment|invoice|travel|processing|instructions)\b",
    flags=re.IGNORECASE,
)

keys_to_fix = []
for key, pt_value in texts_pt.items():
    en_value = texts_en.get(key)
    if not isinstance(pt_value, str) or not isinstance(en_value, str):
        continue
    if pt_value == en_value or english_markers.search(pt_value):
        keys_to_fix.append(key)


def translate_to_ptbr(text: str) -> str:
    url = f"https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=pt&dt=t&q={quote(text)}"
    with urlopen(url, timeout=30) as response:
        data = json.loads(response.read().decode('utf-8'))
    translated_parts = [part[0] for part in data[0] if part and part[0]]
    return ''.join(translated_parts).strip() if translated_parts else text


print(f"Suspect keys to repair: {len(keys_to_fix)}")
repaired = 0
failed = 0

for index, key in enumerate(keys_to_fix, start=1):
    source = texts_en.get(key, '')

    try:
        translated = translate_to_ptbr(source)
        if translated and translated != texts_pt.get(key):
            texts_pt[key] = translated
            repaired += 1
    except Exception as exc:
        failed += 1
        print(f"Failed {index}/{len(keys_to_fix)}: {key} -> {exc}")

    if index % 50 == 0:
        with PT_PATH.open('w', encoding='utf-8') as f:
            json.dump(pt_data, f, ensure_ascii=False, indent=2)
            f.write('\n')
        print(f"Progress {index}/{len(keys_to_fix)}, repaired: {repaired}, failed: {failed}")

    time.sleep(0.05)

with PT_PATH.open('w', encoding='utf-8') as f:
    json.dump(pt_data, f, ensure_ascii=False, indent=2)
    f.write('\n')

print(f"Done. Repaired: {repaired}, Failed: {failed}")
