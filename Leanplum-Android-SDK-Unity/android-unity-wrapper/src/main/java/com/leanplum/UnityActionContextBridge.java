package com.leanplum;

import android.graphics.Color;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.leanplum.callbacks.ActionCallback;
import com.leanplum.internal.FileManager;

import java.util.HashMap;
import java.util.Map;

public class UnityActionContextBridge {

    private static final Gson gson = new Gson();
    public static Map<String, ActionContext> actionContexts = new HashMap<>();

    public static void track(String contextId, String eventName, double value, String params) {
        ActionContext context = actionContexts.get(contextId);
        if (context != null) {
            Map<String, Object> p = gson.fromJson(params, new TypeToken<Map<String, Object>>() {
            }.getType());
            context.track(eventName, value, p);
        }
    }

    public static void trackMessageEvent(String contextId, String eventName, double value, String info, String params) {
        ActionContext context = actionContexts.get(contextId);
        if (context != null) {
            Map<String, Object> p = gson.fromJson(params,
                    new TypeToken<Map<String, Object>>() {
                    }.getType());
            context.trackMessageEvent(eventName, value, info, p);
        }
    }

    public static void runActionNamed(String contextId, String name) {
        ActionContext context = actionContexts.get(contextId);
        if (context != null) {
            context.runActionNamed(name);
        }
    }

    public static void runTrackedActionNamed(String contextId, String name) {
        ActionContext context = actionContexts.get(contextId);
        if (context != null) {
            context.runTrackedActionNamed(name);
        }
    }

    public static String getStringNamed(String contextId, String name) {
        ActionContext context = actionContexts.get(contextId);
        if (context != null) {
            return context.stringNamed(name);
        }
        return null;
    }

    public static boolean getBooleanNamed(String contextId, String name) {
        ActionContext context = actionContexts.get(contextId);
        if (context != null) {
            return context.booleanNamed(name);
        }
        return false;
    }

    public static int getIntNamed(String contextId, String name) {
        ActionContext context = actionContexts.get(contextId);
        if (context != null) {
            Number number = context.numberNamed(name);
            if (number != null) {
                return number.intValue();
            }
        }
        return 0;
    }

    public static float getFloatNamed(String contextId, String name) {
        ActionContext context = actionContexts.get(contextId);
        if (context != null) {
            Number number = context.numberNamed(name);
            if (number != null) {
                return number.floatValue();
            }
        }
        return 0;
    }

    public static double getDoubleNamed(String contextId, String name) {
        ActionContext context = actionContexts.get(contextId);
        if (context != null) {
            Number number = context.numberNamed(name);
            if (number != null) {
                return number.doubleValue();
            }
        }
        return 0;
    }

    public static byte getByteNamed(String contextId, String name) {
        ActionContext context = actionContexts.get(contextId);
        if (context != null) {
            Number number = context.numberNamed(name);
            if (number != null) {
                return number.byteValue();
            }
        }
        return 0;
    }

    public static long getLongNamed(String contextId, String name) {
        ActionContext context = actionContexts.get(contextId);
        if (context != null) {
            Number number = context.numberNamed(name);
            if (number != null) {
                return number.longValue();
            }
        }
        return 0;
    }

    public static short getShortNamed(String contextId, String name) {
        ActionContext context = actionContexts.get(contextId);
        if (context != null) {
            Number number = context.numberNamed(name);
            if (number != null) {
                return number.shortValue();
            }
        }
        return 0;
    }

    public static String getObjectNamed(String contextId, String name) {
        ActionContext context = actionContexts.get(contextId);
        if (context != null) {
            Object obj = context.objectNamed(name);
            if (obj != null) {
                return gson.toJson(obj);
            }
        }
        return null;
    }

    public static int getColorNamed(String contextId, String name) {
        ActionContext context = actionContexts.get(contextId);
        if (context != null) {
            Number n = context.numberNamed(name);
            if (n != null) {
                return n.intValue();
            }
        }
        return Color.BLACK;
    }

    public static String getFileNamed(String contextId, String name) {
        ActionContext context = actionContexts.get(contextId);
        String empty = "";
        if (context != null) {
            String stringValue = context.stringNamed(name);
            if (stringValue == null || stringValue.length() == 0) {
                return empty;
            }
            return FileManager.fileValue(stringValue);
        }
        return empty;
    }

    public static void dismiss(String contextId) {
        ActionContext context = actionContexts.get(contextId);
        if (context != null) {
            context.actionDismissed();
        }
    }
}
