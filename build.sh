#!/usr/bin/env bash
#
# Copyright (c) 2023 Leanplum Inc. All rights reserved.
#

set -o noglob
set -o nounset
set -o pipefail
set -o errexit

PATH_TO_UNITY_ROOT="/Applications/Unity/Unity.app"

UNITY_EDITOR_VERSION=""

XCFRAMEWORKS=("Leanplum" "CleverTapSDK" "SDWebImage")

#######################################
# Gets the latest version of specified repo
# Globals:
#   None
# Arguments:
#   Repo to get from
# Returns:
#   Latest published version
#######################################
get_latest_version() {
    curl --silent "https://api.github.com/repos/$1/releases/latest" | # Get latest release from GitHub api
    grep '"tag_name":' |                                            # Get tag line
    sed -E 's/.*"([^"]+)".*/\1/'                                    # Pluck JSON value
}

#######################################
# Downloads the iOS SDK from GitHub releases.
# Globals:
#   None
# Arguments:
#   The version to download, e.g. "1.2.3"
# Returns:
#   None
#######################################
download_ios_sdk() {
  local version=$1
  local repo=https://github.com/Leanplum/Leanplum-iOS-SDK

  echo "Downloading AppleSDK ${version} ..."
  if [ -d "/tmp/Leanplum-${version}.xcframework" ]; then
    rm -rf "/tmp/Leanplum-${version}.xcframework"
  fi

  # Download from offical git repo.
  local destination="/tmp/Leanplum-${version}.zip"

  # Download the framework zip
  wget --show-progress -O "$destination" \
    "${repo}/releases/download/${version}/Leanplum.zip"

  extract_ios_sdk $version

  echo "Finished downloading iOS SDK."
}



#######################################
# Extracts Leanplum dynamic xcframework and its dependencies.
# Globals:
#   None
# Arguments:
#   The version to use, e.g. "1.2.3"
# Returns:
#   None
#######################################
extract_ios_sdk()
{
  local version=$1
  echo "Extracting AppleSDK ..."
  rm -rf "/tmp/Leanplum"
  rm -rf "/tmp/Leanplum-${version}-dynamic"

  unzip -q "/tmp/Leanplum-${version}.zip" -d "/tmp/Leanplum-${version}"
  mv "/tmp/Leanplum-${version}/dynamic/" "/tmp/Leanplum-${version}-dynamic"

  rm -rf "/tmp/Leanplum-${version}.zip"
  rm -rf "/tmp/Leanplum-${version}"
}

#######################################
# Copies Leanplum dynamic xcframework and its dependencies.
# Globals:
#   XCFRAMEWORKS
# Arguments:
#   The version to use, e.g. "1.2.3"
# Returns:
#   None
#######################################
prepare_ios()
{
  local version=$1
  echo "Preparing iOS dependencies..."

  # Remove all xcframeworks
  find "Leanplum-Unity-SDK/Assets/Plugins/iOS" \
  -name "*.xcframework" \
  -type d \
  -exec sh -c 'if [ -d {} ]; then rm -rf {};fi' \;

  # Add all xcframeworks for specified version
  for i in "${XCFRAMEWORKS[@]}"
  do
    cp -r "/tmp/Leanplum-$version-dynamic/$i.xcframework" \
    "Leanplum-Unity-SDK/Assets/Plugins/iOS/"
  done
}

#######################################
# Copies the iOS SDK from project directory. Name should be in format Leanplum-${version}.zip
# Globals:
#   None
# Arguments:
#   The version to copy.
# Returns:
#   None
#######################################
copy_ios_sdk() {
  local version=$1

  echo "Copying AppleSDK ${version} (Leanplum-${version}.zip)..."
  local destination="/tmp/Leanplum-${version}.zip"

  if [ -d "/tmp/Leanplum-${version}.zip" ]; then
    rm -rf "/tmp/Leanplum-${version}.zip"
  fi

  cp "Leanplum-${version}.zip" $destination

  extract_ios_sdk $version

  echo "Finished copying iOS SDK."
}

#######################################
# Replaces a string in a file and checks for success via git status.
# Globals:
#   None
# Arguments:
#   The path to file.
#   The string to replace.
#   The new string.
# Returns:
#   None
#######################################
replace() {
  sed -i '' -e "s|$2|$3|g" "$1"
  cd "$(dirname "$1")" # Change to directory containing the file and check for success.
  if ! git status --porcelain 2>/dev/null | grep "$(basename "$1")"; then
    echo "${R}Error patching file: $1${N}" && exit 1
  fi
  cd ~- # Change back to original folder.
  echo "Updated file: $1"
}

#######################################
# Tries to find Unity installation path through Unity Hub
# Globals:
#   None
# Returns:
#   None
#######################################
get_unity_from_hub() {
  UNITY_HUB="/Applications/Unity Hub.app/Contents/MacOS/Unity Hub"

  local path=$("${UNITY_HUB}" -- --headless editors -i | head -1 | awk '{print $NF}')
  if [[ $UNITY_EDITOR_VERSION != "" ]]; then
    delimiter="/"
    declare -a array=($(echo $path | tr "$delimiter" " "))
    local count=${#array[@]}
    # the version is second to last element
    array[count-2]=$UNITY_EDITOR_VERSION
    local arr=${array[@]}
    local str=${arr// //}
    path=/$str
  fi
  echo ${path}
}

#######################################
# Builds the Unity SDK.
# Globals:
#   XCFRAMEWORKS
# Arguments:
#   None
# Returns:
#   None
#######################################
build() {
  echo "Building Android SDK..."
  # Build Android SDK
  cd Leanplum-Android-SDK-Unity/
  ./gradlew clean assembleRelease

  CLASSES_JAR=android-unity-wrapper/build/outputs/aar/android-unity-wrapper-release.aar
  UNITY_DIR=Leanplum-Unity-SDK/Assets/Plugins/Android

  cp "${CLASSES_JAR}" "../${UNITY_DIR}/com.leanplum.unity-wrapper-$UNITY_VERSION_STRING.aar"

  cd ../

  echo "Exporting Unity SDK Package..."
  pwd

  if [ -f "$PATH_TO_UNITY_ROOT" ]; then
      echo "$PATH_TO_UNITY_ROOT exist"
  else 
      PATH_TO_UNITY_ROOT=$(get_unity_from_hub)
      echo "Getting Unity from Unity HUB"
  fi

  PATH_TO_UNITY="$PATH_TO_UNITY_ROOT/Contents/MacOS/Unity"
  PATH_TO_PROJECT="$(pwd)/Leanplum-Unity-SDK"
  PATH_TO_EXPORT="$(pwd)/Leanplum-Unity-Package"

  export OUT_PKG="Leanplum_Unity-$UNITY_VERSION_STRING.unitypackage"
  $PATH_TO_UNITY -gvh_disable -quit -nographics -batchmode -projectPath "$PATH_TO_PROJECT" -executeMethod Leanplum.Private.PackageExporter.ExportPackage -logfile
  export UNITY_BINARY="$PATH_TO_PROJECT/$OUT_PKG"

  mv $UNITY_BINARY $PATH_TO_EXPORT

  echo "Done"
}

#######################################
# Configures the Unity SDK build and starts the build.
# Globals:
#   None
# Arguments:
#   None
# Returns:
#   None
#######################################
main() {
  APPLE_COPY=false

  for i in "$@"; do
    case $i in
      --apple-sdk-version=*)
      APPLE_SDK_VERSION="${i#*=}"
      shift # past argument=value
      ;;
      --android-sdk-version=*)
      ANDROID_SDK_VERSION="${i#*=}"
      shift # past argument=value
      ;;
      --ct-android-sdk-version=*)
      CT_ANDROID_SDK_VERSION="${i#*=}"
      shift # past argument=value
      ;;
      --version=*)
      UNITY_VERSION="${i#*=}"
      shift # past argument=value
      ;;
      --unity-editor-version=*)
      UNITY_EDITOR_VERSION="${i#*=}"
      shift # past argument=value
      ;;
      --apple-copy)
      APPLE_COPY=true
      shift
      ;;
      --stacktrace)
      set -o xtrace
      shift
      ;;
    esac
  done

  if [[ -z "${UNITY_VERSION+x}" ]]; then
    echo "Unity SDK version not specified, using current: ${UNITY_VERSION}"
  fi

  if [[ -z ${APPLE_SDK_VERSION+x} ]]; then
    APPLE_SDK_VERSION=$(get_latest_version "Leanplum/Leanplum-iOS-SDK")
    echo "iOS SDK version not specified, using latest: ${APPLE_SDK_VERSION}"
  fi
  if [[ -z ${ANDROID_SDK_VERSION+x} ]]; then
    ANDROID_SDK_VERSION=$(get_latest_version "Leanplum/Leanplum-Android-SDK")
    echo "Android SDK version not specified, using latest: ${ANDROID_SDK_VERSION}"
  fi

  export UNITY_VERSION_STRING=${UNITY_VERSION_STRING:-"$UNITY_VERSION"}
  echo "Building unitypackage with version ${UNITY_VERSION_STRING}, using iOS ${APPLE_SDK_VERSION} and Android ${ANDROID_SDK_VERSION}"

  if [ "$APPLE_COPY" = true ]; then
      copy_ios_sdk $APPLE_SDK_VERSION
  else 
      download_ios_sdk $APPLE_SDK_VERSION
  fi

  replace "Leanplum-Android-SDK-Unity/android-unity-wrapper/build.gradle" "%LP_VERSION%" $ANDROID_SDK_VERSION
  replace "Leanplum-Unity-SDK/Assets/LeanplumSDK/Editor/LeanplumDependencies.xml" "%LP_VERSION%" $ANDROID_SDK_VERSION
  replace "Leanplum-Unity-SDK/Assets/Plugins/Android/mainTemplate.gradle" "%LP_VERSION%" $ANDROID_SDK_VERSION
  replace "Leanplum-Unity-SDK/Assets/Plugins/Android/mainTemplate.gradle" "%CT_VERSION%" $CT_ANDROID_SDK_VERSION
  replace "Leanplum-Android-SDK-Unity/android-unity-wrapper/build.gradle" "%CT_VERSION%" $CT_ANDROID_SDK_VERSION
  replace "Leanplum-Android-SDK-Unity/android-unity-wrapper/build.gradle" "%LP_UNITY_VERSION%" $UNITY_VERSION
  awk -v value="\"$UNITY_VERSION\";" '!x{x=sub(/SDK_VERSION =.*/, "SDK_VERSION = "value)}1' "Leanplum-Unity-SDK/Assets/LeanplumSDK/Utilities/Constants.cs" > Constants_tmp.cs \
    && mv -v Constants_tmp.cs "Leanplum-Unity-SDK/Assets/LeanplumSDK/Utilities/Constants.cs"

  find Leanplum-Unity-Package -name '*.unitypackage' -delete
  find Leanplum-Unity-SDK/Assets/Plugins/ -name '*.aar' -delete

  prepare_ios $APPLE_SDK_VERSION

  build

  git checkout Leanplum-Android-SDK-Unity/
  git checkout Leanplum-Unity-SDK/Assets/LeanplumSDK/Editor/LeanplumDependencies.xml
  git checkout Leanplum-Unity-SDK/Assets/Plugins/Android/mainTemplate.gradle

  echo "Done."
}

main "$@"
