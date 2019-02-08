//
//  Copyright (c) 2013 Leanplum. All rights reserved.
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
package com.leanplum.json;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.util.Log;

import java.util.*;

/**
 * Converts objects to/from JSON.
 * @author Andrew First
 */
public class JsonConverter {
  public static String toJson(Map<String, ?> map) {
    try {
      return mapToJsonObject(map).toString();
    } catch (JSONException e) {
      Log.e("Leanplum", "Error converting " + map + " to JSON", e);
      return null;
    }
  }

  public static Map<String, Object> fromJson(String json) {
    try {
      if (json == null || json.equals("null")) {
        return null;
      }
      return mapFromJson(new JSONObject(json));
    } catch (JSONException e) {
      Log.e("Leanplum", "Error converting " + json + " from JSON", e);
      return null;
    }
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
  public static <T>Map<String, T> mapFromJson(JSONObject object) {
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
}
