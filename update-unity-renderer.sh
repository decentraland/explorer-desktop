#!/bin/bash

# Hack UPM to sync with explorer-desktop with the latest commit on unity-renderer branch, only if exists....

is_unity_local_path()
{
    local UNITYPATH="${1}"
    local FILE="${UNITYPATH}package.json"
    if [ -f "${FILE}" ]; then
        PACKAGE_NAME=$(cat "${FILE}" | jq -r '.name')
        if [ "$PACKAGE_NAME" = "com.decentraland.unity-renderer" ]; then
            cd ${UNITYPATH}
            echo $(pwd)
        fi
    fi
}

search_unity_path()
{
    local UNITYPATH=$1
    [[ $UNITYPATH != *\/ ]] && UNITYPATH="${UNITYPATH}/"
    local PATHFOUND=""
    [[ -z "$PATHFOUND" ]] && PATHFOUND=$(is_unity_local_path ${UNITYPATH})
    [[ -z "$PATHFOUND" ]] && PATHFOUND=$(is_unity_local_path ${UNITYPATH}Assets/)
    [[ -z "$PATHFOUND" ]] && PATHFOUND=$(is_unity_local_path ${UNITYPATH}unity-renderer/Assets/)
    echo ${PATHFOUND}
}

set -e

if [ -z ${1} ]; then
    echo "Usage: $0 <branch>"
    exit
fi

UNITYPATH=$(search_unity_path $1)

if [[ -z "$UNITYPATH" ]]; then
    BRANCH=$1
    echo "Using remote unity-renderer branch=${BRANCH}"

    COMMIT_HASH=$(git ls-remote https://github.com/decentraland/unity-renderer.git ${BRANCH} | awk '{ print $1 }')

    if [ -z ${COMMIT_HASH} ]; then
        # Commit hash not found... using master
        echo "Branch not found. Using unity-renderer remote master"
        BRANCH=master
        COMMIT_HASH=$(git ls-remote https://github.com/decentraland/unity-renderer.git HEAD | awk '{ print $1 }')
    fi

    GIT_URL="git+https://github.com/decentraland/unity-renderer.git?path=unity-renderer/Assets#${BRANCH}"

    # Update package-lock.json
    PACKAGES_LOCK_PATH="unity-renderer-desktop/Packages/packages-lock.json"
    JSON_STR=$(cat ${PACKAGES_LOCK_PATH})
    JSON_STR=$(jq -r '."dependencies"."com.decentraland.unity-renderer"."hash" = $newVal' --arg newVal ${COMMIT_HASH} <<<"$JSON_STR")
    JSON_STR=$(jq -r '."dependencies"."com.decentraland.unity-renderer"."version" = $newVal' --arg newVal ${GIT_URL} <<<"$JSON_STR")
    echo ${JSON_STR} > ${PACKAGES_LOCK_PATH}

    # Update manifest.json
    MANIFEST_LOCK_PATH="unity-renderer-desktop/Packages/manifest.json"
    JSON_STR=$(cat ${MANIFEST_LOCK_PATH})
    JSON_STR=$(jq -r '."dependencies"."com.decentraland.unity-renderer" = $newVal' --arg newVal ${GIT_URL} <<<"$JSON_STR")
    echo "${JSON_STR}" > ${MANIFEST_LOCK_PATH}
else
    LOCAL_URL="file:${UNITYPATH}"
    echo "Using local unity-renderer path=${LOCAL_URL}"

    # Update manifest.json
    MANIFEST_LOCK_PATH="unity-renderer-desktop/Packages/manifest.json"
    JSON_STR=$(cat ${MANIFEST_LOCK_PATH})
    JSON_STR=$(jq -r '."dependencies"."com.decentraland.unity-renderer" = $newVal' --arg newVal ${LOCAL_URL} <<<"$JSON_STR")
    echo "${JSON_STR}" > ${MANIFEST_LOCK_PATH}
fi
