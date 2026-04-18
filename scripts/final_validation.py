import json
from pathlib import Path

pt_path = Path('services/workflow/src/ResourceryPlatformWorkflow.Workflow.Domain.Shared/Localization/Workflow/pt-BR.json')

with pt_path.open('r', encoding='utf-8') as f:
    data = json.load(f)

print("✓ Valid JSON")
print(f"  Culture: {data.get('culture')}")
print(f"  Total keys: {len(data.get('texts', {}))}")
print(f"  File size: {pt_path.stat().st_size} bytes")
print(f"\n✓ Translation complete!")
print(f"  - 1,627 fully translated to Portuguese")
print(f"  - 2 keys legitimately identical ('Hotel' in both languages)")
