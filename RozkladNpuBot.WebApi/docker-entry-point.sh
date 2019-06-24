#!/bin/bash

# is_dev_env=true
# if [[ $ASPNETCORE_ENVIRONMENT = "Docker"]]; then 
#     $is_dev_env=false
# fi

cp /secrets/* /app/Properties/

run_cmd="dotnet RozkladNpuBot.WebApi.dll"
exec $run_cmd





















