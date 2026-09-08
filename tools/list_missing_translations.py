import json
from pathlib import Path
en_path=Path('services/workflow/src/ResourceryPlatformWorkflow.Workflow.Domain.Shared/Localization/Workflow/en.json')
pt_path=Path('services/workflow/src/ResourceryPlatformWorkflow.Workflow.Domain.Shared/Localization/Workflow/pt-BR.json')
base=Path(__file__).resolve().parents[1]
enf=(base/en_path).resolve()
ptf=(base/pt_path).resolve()
with enf.open(encoding='utf-8') as f: en=json.load(f)
with ptf.open(encoding='utf-8') as f: pt=json.load(f)
missing={k:v for k,v in en.get('texts',{}).items() if k not in pt.get('texts',{})}
print(json.dumps(missing,ensure_ascii=False,indent=2))
