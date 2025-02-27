#!/usr/bin/env bash
#
# Copyright (c) 2025 Leanplum Inc. All rights reserved.
#

set -o noglob
set -o nounset
set -o pipefail
set -o errexit

PATH_TO_UNITY_ROOT="/Applications/Unity/Unity.app"
UNITY_HUB="$HOME/Applications/Unity Hub.app/Contents/MacOS/Unity Hub"

# Unity Editor version to be used for building the package. 
# It should be passed as an argument.
UNITY_EDITOR_VERSION=""

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
# Tries to find Unity installation path through Unity Hub.
# Globals:
#   None
# Returns:
#   None
#######################################
get_unity_from_hub() {
  local path=$("${UNITY_HUB}" -- --headless editors -i | grep "$UNITY_EDITOR_VERSION" | awk '{print $NF}')
  if [[ $UNITY_EDITOR_VERSION != "" && $path != "" ]]; then
    local delimiter="/"
    local array=($(echo $path | tr "$delimiter" " "))
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

  echo $PATH_TO_UNITY

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
      --android-sdk-version=*)
      ANDROID_SDK_VERSION="${i#*=}"
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

  if [[ -z ${ANDROID_SDK_VERSION+x} ]]; then
    echo "Leanplum Android SDK version not specified"
    exit
  fi

  if [[ -z "${UNITY_VERSION+x}" ]]; then
    echo "Unity SDK version not specified, using current: ${UNITY_VERSION}"
  fi

  export UNITY_VERSION_STRING=${UNITY_VERSION_STRING:-"$UNITY_VERSION"}

  PACKAGE_IMPORTER_PATH="Leanplum-Unity-SDK/Assets/Editor/PackageImporter.cs"
  CLEVERTAP_UNITY_VERSION=$(sed -n 's/.*CLEVERTAP_UNITY_VERSION = "\(.*\)";/\1/p' "$PACKAGE_IMPORTER_PATH")
  IOS_DEPENDENCIES_PATH="Leanplum-Unity-SDK/Assets/Plugins/Editor/LeanplumDependencies.xml"
  APPLE_SDK_VERSION=$(sed -n 's/.*name="Leanplum-iOS-SDK" version="\([^"]*\)".*/\1/p' "$IOS_DEPENDENCIES_PATH")
  INFO="Building unitypackage with version ${UNITY_VERSION_STRING},
including CleverTapUnity version ${CLEVERTAP_UNITY_VERSION},
using Leanplum iOS ${APPLE_SDK_VERSION} and Leanplum Android ${ANDROID_SDK_VERSION}."
  echo "$INFO"
  sleep 3

  replace "Leanplum-Unity-SDK/Assets/Plugins/Android/leanplum-unity-wrapper.androidlib/build.gradle" "%LP_VERSION%" $ANDROID_SDK_VERSION
  replace "Leanplum-Unity-SDK/Assets/Plugins/Android/leanplum-unity-wrapper.androidlib/build.gradle" "%LP_UNITY_VERSION%" $UNITY_VERSION
  awk -v value="\"$UNITY_VERSION\";" '!x{x=sub(/SDK_VERSION =.*/, "SDK_VERSION = "value)}1' "Leanplum-Unity-SDK/Assets/LeanplumSDK/Utilities/Constants.cs" > Constants_tmp.cs \
    && mv -v Constants_tmp.cs "Leanplum-Unity-SDK/Assets/LeanplumSDK/Utilities/Constants.cs"

  find Leanplum-Unity-Package -name '*.unitypackage' -delete

  build

  git checkout Leanplum-Unity-SDK/Assets/Plugins/Android/leanplum-unity-wrapper.androidlib/build.gradle

  echo "Completed ${INFO}"
  echo "Done."
}

main "$@"
