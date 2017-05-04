#!/bin/bash
cd ../ &&
sudo systemctl stop itsybits.service
git pull origin production
dotnet restore
dotnet ef database update
bower install
sudo systemctl start itsybits.service