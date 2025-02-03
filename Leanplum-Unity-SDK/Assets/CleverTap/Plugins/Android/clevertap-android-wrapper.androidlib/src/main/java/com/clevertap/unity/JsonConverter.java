package com.clevertap.unity;

import android.util.Log;

import androidx.annotation.NonNull;

import com.clevertap.android.sdk.displayunits.model.CleverTapDisplayUnit;
import com.clevertap.android.sdk.usereventlogs.UserEventLog;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

public class JsonConverter {
    private static final String DATE_PREFIX = "ct_date_";

    public static String toJson(Map<String, ?> map) {
        try {
            return mapToJsonObject(map).toString();
        } catch (JSONException e) {
            Log.e("CleverTap", "Error converting " + map + " to JSON", e);
            return null;
        }
    }

    public static Map<String, Object> fromJsonWithConvertedDateValues(String json) {
        Map<String, Object> jsonMap = JsonConverter.fromJson(json);
        if (jsonMap == null) {
            return null;
        }

        for (String key : jsonMap.keySet()) {
            Object value = jsonMap.get(key);
            if (value instanceof String) {
                String str = (String) value;
                int index = str.indexOf(DATE_PREFIX);
                if (index != -1) {
                    try {
                        String tsSubstring = str.substring(index + DATE_PREFIX.length());
                        long ts = Long.parseLong(tsSubstring);
                        Date date = new Date(ts);
                        jsonMap.put(key, date);
                    } catch (Exception e) {
                        Log.e("CleverTap", "Failed to parse date: " + str, e);
                    }
                }
            }
        }
        return jsonMap;
    }

    public static Map<String, Object> fromJson(String json) {
        try {
            if (json == null || json.equals("null")) {
                return null;
            }
            return mapFromJson(new JSONObject(json));
        } catch (JSONException e) {
            Log.e("CleverTap", "Error converting " + json + " from JSON", e);
            return null;
        }
    }

    @NonNull
    public static <T> JSONObject mapToJson(Map<String, T> map, @NonNull Function.Transformer<T, Object> mapper) {
        JSONObject json = new JSONObject();
        if (map == null) {
            return json;
        }

        for (String key : map.keySet()) {
            Object value = mapper.apply(map.get(key));
            try {
                json.put(key, value);
            } catch (JSONException e) {
                Log.e("CleverTap", "Error converting map to JSON. key: " + key + " value: " + value, e);
            }
        }

        return json;
    }

    @SuppressWarnings("unchecked")
    public static JSONObject mapToJsonObject(Map<String, ?> map) throws JSONException {
        JSONObject obj = new JSONObject();
        for (String key : map.keySet()) {
            Object value = map.get(key);
            if (value instanceof Map) {
                value = mapToJsonObject((Map<String, ?>) value);
            } else if (value instanceof Iterable) {
                value = listToJsonArray((Iterable<?>) value);
            }
            obj.put(key, value);
        }
        return obj;
    }

    @SuppressWarnings("unchecked")
    public static JSONArray listToJsonArray(Iterable<?> list) throws JSONException {
        JSONArray obj = new JSONArray();
        for (Object value : list) {
            if (value instanceof Map) {
                value = mapToJsonObject((Map<String, ?>) value);
            } else if (value instanceof Iterable) {
                value = listToJsonArray((Iterable<?>) value);
            }
            obj.put(value);
        }
        return obj;
    }

    @SuppressWarnings("unchecked")
    public static <T> Map<String, T> mapFromJson(JSONObject object) {
        if (object == null) {
            return null;
        }
        Iterator<?> keysIterator = object.keys();
        Map<String, T> result = new HashMap<String, T>();
        while (keysIterator.hasNext()) {
            String key = (String) keysIterator.next();
            Object value = object.opt(key);
            if (value == null || value == JSONObject.NULL) {
                value = null;
            } else if (value instanceof JSONObject) {
                value = mapFromJson((JSONObject) value);
            } else if (value instanceof JSONArray) {
                value = listFromJson((JSONArray) value);
            }
            result.put(key, (T) value);
        }
        return result;
    }

    public static List<Object> listFromJson(JSONArray json) {
        if (json == null) {
            return null;
        }
        List<Object> result = new ArrayList<Object>(json.length());
        for (int i = 0; i < json.length(); i++) {
            Object value = json.opt(i);
            if (value == null || value == JSONObject.NULL) {
                value = null;
            } else if (value instanceof JSONObject) {
                value = mapFromJson((JSONObject) value);
            } else if (value instanceof JSONArray) {
                value = listFromJson((JSONArray) value);
            }
            result.add(value);
        }
        return result;
    }

    public static JSONArray displayUnitListToJSONArray(ArrayList<CleverTapDisplayUnit> displayUnits)
            throws JSONException {
        JSONArray array = new JSONArray();

        for (int i = 0; i < displayUnits.size(); i++) {
            array.put(displayUnits.get(i).getJsonObject());
        }

        return array;
    }

    @NonNull
    public static JSONObject userEventLogToJSON(UserEventLog eventLog) {
        JSONObject json = new JSONObject();
        if (eventLog == null) {
            return json;
        }

        try {
            json.put("eventName", eventLog.getEventName());
            json.put("normalizedEventName", eventLog.getNormalizedEventName());
            json.put("firstTs", eventLog.getFirstTs());
            json.put("lastTs", eventLog.getLastTs());
            json.put("countOfEvents", eventLog.getCountOfEvents());
            json.put("deviceID", eventLog.getDeviceID());
        } catch (JSONException e) {
            Log.e("CleverTap", "Error converting eventLog to JSON: " + eventLog, e);
        }
        return json;
    }
}
