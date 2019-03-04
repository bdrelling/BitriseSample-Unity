#!/usr/bin/env bash
set -x

# We don't need to do all this Cert stuff locally
if [ -z $IS_CI ]; then
  # Initialize CACerts.pem
  /Applications/Unity/Unity.app/Contents/MacOS/Unity -logfile & 
  sleep 15
  sudo killall Unity

  /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode \
    -serial $UNITY_SERIAL \
    -username $UNITY_EMAIL \
    -password $UNITY_PASSWORD \
    -logfile
else
  echo 'Detected local environment. Skipping cert and license initialization.'
fi

/Applications/Unity/Unity.app/Contents/MacOS/Unity -nographics -quit -batchmode -logFile \
  -projectPath $UNITY_PROJECT_PATH \
  -executeMethod $UNITY_EXECUTE_METHOD \
  -output $UNITY_OUTPUT_DIRECTORY \
  -target ios

# /Applications/Unity/Unity.app/Contents/MacOS/Unity -nographics -quit -batchmode -logFile -projectPath "$BITRISE_SOURCE_DIR" -executeMethod $UNITY_EXECUTE_METHOD -androidSdkPath "$ANDROID_HOME" -buildOutput "$BITRISE_DEPLOY_DIR/mygame.apk" -buildPlatform android