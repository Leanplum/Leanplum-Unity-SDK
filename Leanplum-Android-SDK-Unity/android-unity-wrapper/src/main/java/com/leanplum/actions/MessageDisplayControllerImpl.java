/*
 * Copyright 2022, Leanplum, Inc. All rights reserved.
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

package com.leanplum.actions;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import com.leanplum.ActionContext;
import com.leanplum.ActionContext.ContextualValues;
import com.leanplum.IJavaBridge;
import com.leanplum.UnityActionContextBridge;
import com.leanplum.UnityBridge;
import com.leanplum.actions.internal.ActionsTrigger;
import com.leanplum.json.JsonConverter;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

public class MessageDisplayControllerImpl implements MessageDisplayController {

  private final IJavaBridge javaBridge;

  public MessageDisplayControllerImpl(@NonNull IJavaBridge javaBridge) {
    this.javaBridge = javaBridge;
  }

  private String convertActionsTrigger(@Nullable ActionsTrigger actionsTrigger) {
    Map<String, Object> jsonMap = new HashMap<>();

    String eventName = "";
    if (actionsTrigger != null && actionsTrigger.getEventName() != null) {
      eventName = actionsTrigger.getEventName();
    }

    List<String> condition = new ArrayList<>();
    if (actionsTrigger != null && actionsTrigger.getCondition() != null) {
      condition = actionsTrigger.getCondition();
    }

    Map<String, Object> contextualMap = new HashMap<>();
    if (actionsTrigger != null && actionsTrigger.getContextualValues() != null) {
      ContextualValues contextualValues = actionsTrigger.getContextualValues();
      contextualMap.put("parameters", contextualValues.parameters);
      contextualMap.put("arguments", contextualValues.arguments);
      contextualMap.put("previousAttributeValue", contextualValues.previousAttributeValue);
      contextualMap.put("attributeValue", contextualValues.attributeValue);
    }

    jsonMap.put("eventName", eventName);
    jsonMap.put("condition", condition);
    jsonMap.put("contextualValues", contextualMap);

    return JsonConverter.toJson(jsonMap);
  }

  @NonNull
  @Override
  public List<ActionContext> prioritizeMessages(
      @NonNull List<? extends ActionContext> list,
      @Nullable ActionsTrigger actionsTrigger) {

    String actionsTriggerJson = convertActionsTrigger(actionsTrigger);

    StringBuilder sb = new StringBuilder();
    for (ActionContext ac: list) {
      String contextId = UnityBridge.getActionContextId(ac);
      UnityActionContextBridge.actionContexts.put(contextId, ac);
      if (sb.length() == 0) {
        sb.append(contextId);
      } else {
        sb.append(',');
        sb.append(contextId);
      }
    }
    String csv = javaBridge.prioritizeMessages(sb.toString(), actionsTriggerJson);
    String[] contextIds = csv.split(",");

    List<ActionContext> prioritized = new LinkedList<>();
    for (String id: contextIds) {
      ActionContext ac = UnityActionContextBridge.actionContexts.get(id);
      if (ac != null) {
        prioritized.add(ac);
      }
    }
    return prioritized;
  }

  @Nullable
  @Override
  public MessageDisplayChoice shouldDisplayMessage(@NonNull ActionContext actionContext) {
    String contextId = UnityBridge.getActionContextId(actionContext);
    int result = javaBridge.shouldDisplayMessage(contextId);
    switch (result) {
      case 0:
        return MessageDisplayChoice.show();
      case 1:
        return MessageDisplayChoice.discard();
      case -1:
        return MessageDisplayChoice.delayIndefinitely();
      default:
        int delay = result - 2;
        return MessageDisplayChoice.delay(delay);
    }
  }
}
