#!/usr/bin/env bash

source ci-setup.sh

echo Downloading AVProVideo

echo "${GPG_PRIVATE_KEY_BASE64}" | base64 -d > private.gpg
gpg  --batch --import private.gpg
curl -L 'https://renderer-artifacts.decentraland.org/artifacts/Custom_AVProVideo_2.6.7_ULTRA.unitypackage.gpg' -o Custom_AVProVideo_2.6.7_ULTRA.unitypackage.gpg

gpg --output Custom_AVProVideo_2.6.7_ULTRA.unitypackage --decrypt Custom_AVProVideo_2.6.7_ULTRA.unitypackage.gpg

echo Finished downloading AVProVideo

echo Begin importing AVProVideo

xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' $UNITY_PATH/Editor/Unity \
  -quit \
  -batchmode \
  -importPackage $(pwd)/Custom_AVProVideo_2.6.7_ULTRA.unitypackage \
  -projectPath "$PROJECT_PATH"
  
echo Ended importing AvProVideo

echo Begin importing license

if [ -z "$DEVELOPERS_UNITY_LICENSE_CONTENT_2020_3_BASE64" ]; then
  echo 'DEVELOPERS_UNITY_LICENSE_CONTENT_2020_3_BASE64 not present. License won''t be configured'
else
  LICENSE=$(echo "${DEVELOPERS_UNITY_LICENSE_CONTENT_2020_3_BASE64}" | base64 -di | tr -d '\r')

  echo "Writing LICENSE to license file /root/.local/share/unity3d/Unity/Unity_lic.ulf"
  echo "$LICENSE" > /root/.local/share/unity3d/Unity/Unity_lic.ulf
fi

echo End importing license

echo "Building for $BUILD_TARGET at $PROJECT_PATH"

xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' $UNITY_PATH/Editor/Unity \
  -quit \
  -batchmode \
  -projectPath "$PROJECT_PATH" \
  -logFile "$BUILD_PATH/build-logs.txt" \
  -buildTarget "$BUILD_TARGET" \
  -customBuildTarget "$BUILD_TARGET" \
  -customBuildName "$BUILD_NAME" \
  -customBuildPath "$BUILD_PATH" \
  -executeMethod BuildCommand.PerformBuild

UNITY_EXIT_CODE=$?

if [ $UNITY_EXIT_CODE -eq 0 ]; then
  echo "Run succeeded, no failures occurred";
elif [ $UNITY_EXIT_CODE -eq 2 ]; then
  echo "Run succeeded, some tests failed";
elif [ $UNITY_EXIT_CODE -eq 3 ]; then
  echo "Run failure (other failure)";
else
  echo "Unexpected exit code $UNITY_EXIT_CODE";
fi

ls -la "$BUILD_PATH"

if [ -z "$(ls -A "$BUILD_PATH")" ]; then
  echo "directory BUILD_PATH $BUILD_PATH is empty"
  UNITY_EXIT_CODE=4
fi

exit $UNITY_EXIT_CODE;
