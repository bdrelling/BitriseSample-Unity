#!/usr/bin/env bash

# 2018.3.5f1 / 76b3e37670a4

if [ ! -f ./unity.pkg ]; then
    # Latest: https://download.unity3d.com/download_unity/MacEditorInstaller/Unity.pkg
    url="https://download.unity3d.com/download_unity/${UNITY_INSTALLER_ID}/MacEditorInstaller/Unity.pkg"
    echo "Downloading Unity package from $url"
    curl -o ./unity.pkg $url
fi

if [ ! -f ./ios.pkg ]; then
    # Latest: https://download.unity3d.com/download_unity/MacEditorTargetInstaller/UnitySetup-iOS-Support-for-Editor.pkg
    #url="https://download.unity3d.com/download_unity/${UNITY_INSTALLER_ID}/MacEditorTargetInstaller/UnitySetup-iOS-Support-for-Editor-${UNITY_VERSION}.pkg"
    url="https://download.unity3d.com/download_unity/${UNITY_INSTALLER_ID}/MacEditorTargetInstaller/UnitySetup-iOS-Support-for-Editor.pkg"
    echo "Downloading Unity iOS Target package from $url"
    curl -o ./ios.pkg $url
fi

#if [ ! -f ./android.pkg ]; then
    #curl -o ./android.pkg "https://download.unity3d.com/download_unity/${UNITY_INSTALLER_ID}/MacEditorTargetInstaller/UnitySetup-Android-Support-for-Editor.pkg"
#fi