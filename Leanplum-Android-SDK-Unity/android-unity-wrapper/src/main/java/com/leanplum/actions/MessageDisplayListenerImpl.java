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
import com.leanplum.ActionContext;
import com.leanplum.IJavaBridge;
import com.leanplum.UnityBridge;

public class MessageDisplayListenerImpl implements MessageDisplayListener {

  private final IJavaBridge javaBridge;

  public MessageDisplayListenerImpl(@NonNull IJavaBridge javaBridge) {
    this.javaBridge = javaBridge;
  }

  @Override
  public void onActionExecuted(@NonNull String name, @NonNull ActionContext actionContext) {
    String contextKey = UnityBridge.getActionContextKey(actionContext);
    javaBridge.onMessageAction(name, contextKey);
  }

  @Override
  public void onMessageDismissed(@NonNull ActionContext actionContext) {
    String contextKey = UnityBridge.getActionContextKey(actionContext);
    javaBridge.onMessageDismissed(contextKey);
  }

  @Override
  public void onMessageDisplayed(@NonNull ActionContext actionContext) {
    String contextKey = UnityBridge.getActionContextKey(actionContext);
    javaBridge.onMessageDisplayed(contextKey);
  }
}
