package com.clevertap.unity;

import android.os.Handler;
import android.os.Looper;

import java.util.concurrent.Executor;
import java.util.concurrent.Executors;

public class BackgroundExecutor {

    private final Executor executor = Executors.newSingleThreadExecutor(runnable -> {
        Thread newThread = Executors.defaultThreadFactory().newThread(runnable);
        newThread.setName("CleverTapUnityPluginBackgroundThread");
        return newThread;
    });

    private final Handler mainHandler = new Handler(Looper.getMainLooper());

    /**
     * Execute work in a background thread and publish the result on the main thread.
     */
    public <T> void execute(final Function.Producer<T> backgroundWork,
                            final Function.Consumer<T> resultCallback) {
        executor.execute(() -> {
            T result = backgroundWork.apply();
            mainHandler.post(() -> resultCallback.apply(result));
        });
    }
}
