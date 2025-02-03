package com.clevertap.unity;

import android.util.Log;

import androidx.annotation.NonNull;

import com.unity3d.player.UnityPlayer;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.Map;
import java.util.Queue;

/**
 * CleverTapMessageSender is responsible for sending messages to the Unity callback handler.
 * It manages message buffers which is useful when needing to send a message while the Unity runtime
 * of the application is not yet initialized. Only callbacks specified as
 * {@link CleverTapUnityCallback#bufferable bufferable} will be considered for buffering, all other
 * message callbacks will be send immediately.
 */
public class CleverTapMessageSender {
    private static final String LOG_TAG = "CleverTapMessageSender";
    private static final String CLEVERTAP_GAME_OBJECT_NAME = "AndroidCallbackHandler";

    private static CleverTapMessageSender instance = null;

    public synchronized static CleverTapMessageSender getInstance() {
        if (instance == null) {
            instance = new CleverTapMessageSender();
        }
        return instance;
    }

    private Map<CleverTapUnityCallback, MessageBuffer> messageBuffers;

    private CleverTapMessageSender() {
        messageBuffers = createBuffersMap(true);
    }

    /**
     * Enable buffering for the specified callback. While enabled, all messages which should be send
     * will be added to the buffer. Only buffers for callbacks specified as
     * {@link CleverTapUnityCallback#bufferable buferrable} can be enabled.
     *
     * @see #disableBuffer(CleverTapUnityCallback)
     */
    public synchronized void enableBuffer(@NonNull CleverTapUnityCallback callback) {
        final MessageBuffer messageBuffer = messageBuffers.get(callback);
        if (messageBuffer != null) {
            messageBuffer.enabled = true;
        }
    }

    /**
     * Disable buffering for the specified callback. This will only have effect for events specified
     * as {@link CleverTapUnityCallback#bufferable buferrable}
     *
     * @see #enableBuffer(CleverTapUnityCallback)
     */
    public synchronized void disableBuffer(@NonNull CleverTapUnityCallback callback) {
        final MessageBuffer messageBuffer = messageBuffers.get(callback);
        if (messageBuffer != null) {
            messageBuffer.enabled = false;
        }
    }

    /**
     * Clear all the buffered messages from all buffers and set whether all buffers should be enabled
     * after that.
     *
     * @param enableBuffers enable/disable all buffers after they are cleared
     */
    public synchronized void resetAllBuffers(boolean enableBuffers) {
        messageBuffers = createBuffersMap(enableBuffers);
        Log.i(LOG_TAG, "Message buffers reset and enabled: " + enableBuffers);
    }

    /**
     * Send all currently buffered messages for the specified callback.
     */
    public synchronized void flushBuffer(@NonNull CleverTapUnityCallback callback) {
        final MessageBuffer messageBuffer = messageBuffers.get(callback);
        if (messageBuffer == null) {
            return;
        }

        while (messageBuffer.size() > 0) {
            String messageData = messageBuffer.remove();
            sendMessage(callback, messageData);
        }
    }

    /**
     * Send a message to the specified callback. It will be buffered if buffering for the callback
     * is enabled or it will be send immediately otherwise.
     *
     * @param callback    The callbacks to message
     * @param messageData The message data
     * @see #enableBuffer(CleverTapUnityCallback)
     * @see #disableBuffer(CleverTapUnityCallback)
     */
    public synchronized void send(@NonNull CleverTapUnityCallback callback, @NonNull String messageData) {
        final MessageBuffer messageBuffer = messageBuffers.get(callback);
        if (messageBuffer != null && messageBuffer.enabled) {
            addToBuffer(callback, messageData);
        } else {
            sendMessage(callback, messageData);
        }
    }

    /**
     * Adds a message to the callback's buffer for future sending.
     * Messages will remain in the buffer until {@link #flushBuffer(CleverTapUnityCallback)} for the
     * same callback name is called.
     *
     * @param callback    The callback to message
     * @param messageData The message data to be sent
     */
    private void addToBuffer(@NonNull CleverTapUnityCallback callback, @NonNull String messageData) {
        final MessageBuffer messageBuffer = messageBuffers.get(callback);
        if (messageBuffer == null) {
            return;
        }

        messageBuffer.add(messageData);
        Log.i(LOG_TAG, "Message buffered " + callback.callbackName + ":" + messageData);
    }

    private void sendMessage(@NonNull CleverTapUnityCallback callback, @NonNull String data) {
        switch (callback.mode) {
            case UNITY_PLAYER_MESSAGE: {
                try {
                    UnityPlayer.UnitySendMessage(CLEVERTAP_GAME_OBJECT_NAME, callback.callbackName, data);
                } catch (RuntimeException e) {
                    Log.e(LOG_TAG, "Failed to send message to Unity.", e);
                }
                break;
            }
            case DIRECT_CALLBACK: {
                if (callback.pluginCallback == null) {
                    Log.d(LOG_TAG, "Message not sent, direct callback is null for " + callback.callbackName + ":" + data);
                    return;
                }
                callback.pluginCallback.Invoke(data);
                break;
            }
        }
        Log.i(LOG_TAG, "Message sent " + callback.callbackName + ":" + data);
    }

    private Map<CleverTapUnityCallback, MessageBuffer> createBuffersMap(boolean enableBuffers) {
        final Map<CleverTapUnityCallback, MessageBuffer> map = new HashMap<>();
        for (CleverTapUnityCallback callback : CleverTapUnityCallback.values()) {
            if (callback.bufferable) {
                map.put(callback, new MessageBuffer(enableBuffers));
            }
        }
        return map;
    }

    private static class MessageBuffer {
        private boolean enabled;
        private final Queue<String> items;

        MessageBuffer(boolean enabled) {
            this.enabled = enabled;
            items = new LinkedList<>();
        }

        public void add(String item) {
            items.add(item);
        }

        public String remove() {
            return items.remove();
        }

        public int size() {
            return items.size();
        }
    }
}
