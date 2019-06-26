#!/bin/bash

rm -r /rozklad-app
mkdir /rozklad-app
cp ./docker-compose.yml /rozklad-app/
cp -r ./env-files /rozklad-app/


rm -r /rozklad-app-nginx-certs
cp -r ./rozklad-app-nginx-certs /


rm -r /rozklad-app-secrets
cp -r ./rozklad-app-secrets /


