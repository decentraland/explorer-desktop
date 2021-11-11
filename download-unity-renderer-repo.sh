#!/bin/bash

get_unity_renderer_hash()
{
    # Update package-lock.json
    PACKAGES_LOCK_PATH="unity-renderer-desktop/Packages/packages-lock.json"
    JSON_STR=$(cat ${PACKAGES_LOCK_PATH})
    JSON_STR=$(jq -r '."dependencies"."com.decentraland.unity-renderer"."hash"'  <<< "$JSON_STR")

    echo ${JSON_STR}
}

UNITY_RENDERER_HASH=$(get_unity_renderer_hash)
echo "Downloading unity-renderer hash=${UNITY_RENDERER_HASH}..."

[[ -d "./unity-renderer" ]] && rm -rf ./unity-renderer # If exists, delete and download again
git clone https://github.com/decentraland/unity-renderer
pushd .
cd unity-renderer
git checkout ${UNITY_RENDERER_HASH}
popd

# Set local repo
./update-unity-renderer.sh unity-renderer