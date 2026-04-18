import json
import re
from pathlib import Path

ROOT = Path('services/workflow/src/ResourceryPlatformWorkflow.Workflow.Domain.Shared/Localization/Workflow')
EN_PATH = ROOT / 'en.json'
PT_PATH = ROOT / 'pt-BR.json'

with EN_PATH.open('r', encoding='utf-8') as f:
    en_data = json.load(f)

with PT_PATH.open('r', encoding='utf-8') as f:
    pt_data = json.load(f)

texts_en = en_data.get('texts', {})
texts_pt = pt_data.get('texts', {})

# Find entries identical to English
identical = [k for k in texts_pt if texts_pt[k] == texts_en.get(k)]

# Find entries containing English marker words
english_markers = re.compile(
    r"\b(the|with|from|request|office|director|staff|meeting|draft|approved|submit|review|once|upon|while|through|should|will|can|delivered|dispatch|output|feedback|copy|hard|soft|check|follow-up|supporting|documents|team|service|payment|invoice|travel|processing|instructions)\b",
    flags=re.IGNORECASE,
)
mixed_english = [k for k in texts_pt if english_markers.search(texts_pt.get(k, ''))]

print(f"Total keys: {len(texts_pt)}")
print(f"Identical to English: {len(identical)}")
print(f"Contains English markers: {len(mixed_english)}")
print(f"Translation complete: {len(identical) == 0 and len(mixed_english) == 0}")

if identical:
    print("\n=== Identical to English (first 10) ===")
    for k in identical[:10]:
        print(f"  {k}")

if mixed_english:
    print("\n=== Still contains English (first 10) ===")
    for k in mixed_english[:10]:
        print(f"  {k}: {texts_pt[k][:100]}")
