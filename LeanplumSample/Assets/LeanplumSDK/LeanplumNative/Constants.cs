//
// Copyright 2013, Leanplum, Inc.
//
//  Licensed to the Apache Software Foundation (ASF) under one
//  or more contributor license agreements.  See the NOTICE file
//  distributed with this work for additional information
//  regarding copyright ownership.  The ASF licenses this file
//  to you under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing,
//  software distributed under the License is distributed on an
//  "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//  KIND, either express or implied.  See the License for the
//  specific language governing permissions and limitations
//  under the License.
namespace LeanplumSDK
{
    /// <summary>
    ///     Leanplum constants.
    /// </summary>
    public class Constants
    {
        internal const string CLIENT_PREFIX = "unity";
        internal const string EVENT_EXCEPTION = "__exception";
        internal static string API_HOST_NAME = "www.leanplum.com";
        internal static string SOCKET_HOST = "dev.leanplum.com";
        internal static int SOCKET_PORT = 80;
        internal static bool API_SSL = true;
        internal static int NETWORK_TIMEOUT_SECONDS = 10;
        internal static int NETWORK_TIMEOUT_SECONDS_FOR_DOWNLOADS = 10;
        internal static int NETWORK_SOCKET_TIMEOUT_SECONDS = 10;
        internal const string NETWORK_TIMEOUT_MESSAGE = "Request timed out.";

        internal static bool isDevelopmentModeEnabled = false;
        internal static bool isInPermanentFailureState = false;
        internal static bool loggingEnabled = false;
        internal static bool isNoop = false;
#if UNITY_WEBGL
        internal static bool EnableRealtimeUpdatesInDevelopmentMode = false;
#else
        internal static bool EnableRealtimeUpdatesInDevelopmentMode = true;
#endif

        internal static string API_SERVLET = "api";

        internal const int MAX_STORED_API_CALLS = 10000;

        internal class Crypt
        {
            internal const int ITER_COUNT = 1000; // TODO: too many? set to a lower num, like 2?
            internal const int KEY_LENGTH = 256;
            internal const string SALT = "L3@nP1Vm"; // Must have 8 bytes
            internal const string VECTOR = "__l3anplum__iv__"; // Must have 16 bytes
        }

        internal class Defaults
        {
            internal const string COUNT_KEY = "__leanplum_unsynced";
            internal const string START_KEY = "__leanplum_unsynced_start";
            internal const string ITEM_KEY = "__leanplum_unsynced_{0}";
            internal const string VARIABLES_KEY = "__leanplum_variables";
            internal const string FILE_ATTRIBUTES_KEY = "__leanplum_file_attributes";
            internal const string TOKEN_KEY = "__leanplum_token";
            internal const string USERID_KEY = "___leanplum_userid";
        }

        internal class Files
        {
            internal const int MAX_UPLOAD_BATCH_FILES = 16;
            internal static readonly int MAX_UPLOAD_BATCH_SIZES = (25 * (1 << 20));
        }

        internal class Keys
        {
            internal const string CITY = "city";
            internal const string COUNTRY = "country";
            internal const string ERROR = "error";
            internal const string FILENAME = "filename";
            internal const string FILE_ATTRIBUTES = "fileAttributes";
            internal const string HASH = "hash";
            internal const string IS_REGISTERED = "isRegistered";
            internal const string LATEST_VERSION = "latestVersion";
            internal const string LOCALE = "locale";
            internal const string LOCATION = "location";
            internal const string MESSAGE = "message";
            internal const string REASON = "reason";
            internal const string RESPONSE = "response";
            internal const string REGION = "region";
            internal const string SIZE = "size";
            internal const string STACK_TRACE = "stackTrace";
            internal const string SUCCESS = "success";
            internal const string TIMEZONE = "timezone";
            internal const string TIMEZONE_OFFSET_SECONDS = "timezoneOffsetSeconds";
            internal const string TOKEN = "token";
            internal const string VARS_FROM_CODE = "varsFromCode";
            internal const string USER_INFO = "userInfo";
            internal const string URL = "url";
            internal const string VARIANTS = "variants";
            internal const string VARS = "vars";
        }

        public class Kinds
        {
            public const string INT = "integer";
            public const string FLOAT = "float";
            public const string STRING = "string";
            public const string BOOLEAN = "bool";
            public const string FILE = "file";
            public const string DICTIONARY = "group";
            public const string ARRAY = "list";
        }

        internal class Methods
        {
            internal const string ADVANCE = "advance";
            internal const string DOWNLOAD_FILE = "downloadFile";
            internal const string GET_VARS = "getVars";
            internal const string MULTI = "multi";
            internal const string PAUSE_SESSION = "pauseSession";
            internal const string PAUSE_STATE = "pauseState";
            internal const string REGISTER_FOR_DEVELOPMENT = "registerDevice";
            internal const string RESUME_SESSION = "resumeSession";
            internal const string RESUME_STATE = "resumeState";
            internal const string SET_USER_ATTRIBUTES = "setUserAttributes";
            internal const string SET_VARS = "setVars";
            internal const string START = "start";
            internal const string STOP = "stop";
            internal const string TRACK = "track";
            internal const string UPLOAD_FILE = "uploadFile";
            internal const string SET_TRAFFIC_SOURCE_INFO = "setTrafficSourceInfo";
        }

        internal class Params
        {
            internal const string ACTION = "action";
            internal const string APP_ID = "appId";
            internal const string CLIENT = "client";
            internal const string CLIENT_KEY = "clientKey";
            internal const string DATA = "data";
            internal const string DEV_MODE = "devMode";
            internal const string DEVICE_ID = "deviceId";
            internal const string DEVICE_MODEL = "deviceModel";
            internal const string DEVICE_NAME = "deviceName";
            internal const string DEVICE_SYSTEM_NAME = "systemName";
            internal const string DEVICE_SYSTEM_VERSION = "systemVersion";
            internal const string EMAIL = "email";
            internal const string EVENT = "event";
            internal const string FILE = "file";
            internal const string INCLUDE_DEFAULTS = "includeDefaults";
            internal const string INFO = "info";
            internal const string KINDS = "kinds";
            internal const string NEW_USER_ID = "newUserId";
            internal const string PARAMS = "params";
            internal const string SDK_VERSION = "sdkVersion";
            internal const string STATE = "state";
            internal const string TIME = "time";
            internal const string TOKEN = "token";
            internal const string USER_ID = "userId";
            internal const string USER_ATTRIBUTES = "userAttributes";
            internal const string VALUE = "value";
            internal const string VARIABLES = "variables";
            internal const string VERSION_CODE = "versionCode";
            internal const string VERSION_NAME = "versionName";
            internal const string TRAFFIC_SOURCE = "trafficSource";
        }

        public class Values
        {
            public const string DETECT = "(detect)";
            public const string ACTION_PREFIX = "__action__";
            public const string RESOURCES_VARIABLE = "__Unity Resources";
        }
    }
}
