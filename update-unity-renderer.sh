#!/usr/bin/env bash

# Hack UPM to sync with explorer-desktop with the latest commit on unity-renderer branch, only if exists...

set -e

if [ -z ${1} ]; then
    echo "Usage: $0 <branch>"
    exit
fi

BRANCH=$1

COMMIT_HASH=$(git ls-remote https://github.com/decentraland/unity-renderer.git ${BRANCH} | awk '{ print $1 }')

if [ -z ${COMMIT_HASH} ]; then
    # Commit hash not found... using master
    BRANCH=master
    COMMIT_HASH=$(git ls-remote https://github.com/decentraland/unity-renderer.git HEAD | awk '{ print $1 }')
fi

GIT_URL="git+https://github.com/decentraland/unity-renderer.git?path=unity-renderer/Assets#${BRANCH}"

# Update package-lock.json
PACKAGES_LOCK_PATH="unity-renderer-desktop/Packages/packages-lock.json"
JSON_STR=$(cat ${PACKAGES_LOCK_PATH})
JSON_STR=$(jq '."dependencies"."com.decentraland.unity-renderer"."hash" = $newVal' --arg newVal ${COMMIT_HASH} <<<"$JSON_STR")
JSON_STR=$(jq '."dependencies"."com.decentraland.unity-renderer"."version" = $newVal' --arg newVal ${GIT_URL} <<<"$JSON_STR")
echo ${JSON_STR} > ${PACKAGES_LOCK_PATH}

# Update manifest.json
MANIFEST_LOCK_PATH="unity-renderer-desktop/Packages/manifest.json"
JSON_STR=$(cat ${MANIFEST_LOCK_PATH})
JSON_STR=$(jq '."dependencies"."com.decentraland.unity-renderer" = $newVal' --arg newVal ${GIT_URL} <<<"$JSON_STR")
echo ${JSON_STR} > ${MANIFEST_LOCK_PATH}