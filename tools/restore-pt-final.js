const fs=require('fs'),exec=require('child_process').execFileSync;
const rel='services/administration/src/ResourceryPlatformWorkflow.Administration.Domain.Shared/Localization/Administration/pt-BR.json';
const head=JSON.parse(exec('git',['show','HEAD:'+rel],{encoding:'utf8'}).replace(/^\uFEFF/,''));
const work=JSON.parse(fs.readFileSync(rel,'utf8'));
const out={culture:'pt-BR',texts:{...head.texts,...work.texts}};
fs.writeFileSync(rel,JSON.stringify(out,null,4)+'\n');
console.log(Object.keys(out.texts).length);
