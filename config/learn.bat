@echo off
call C:\Users\phili\Documentos\Desenvolvimento\ml-agents\python-envs\penv\Scripts\activate
cd /d "C:\Users\phili\Documentos\Desenvolvimento\Games\Unity\Deep Blue\config"
mlagents-learn ant-config.yaml --run-id=exp1 --force
pause