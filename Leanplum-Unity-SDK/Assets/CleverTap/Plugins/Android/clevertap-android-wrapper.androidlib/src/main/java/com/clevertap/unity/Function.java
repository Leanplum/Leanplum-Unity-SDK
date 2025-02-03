package com.clevertap.unity;


public interface Function {

    @FunctionalInterface
    interface Producer<T> {
        T apply();
    }

    @FunctionalInterface
    interface Consumer<T> {
        void apply(T param);
    }

    @FunctionalInterface
    interface Transformer<T, R> {
        R apply(T param);
    }

}
