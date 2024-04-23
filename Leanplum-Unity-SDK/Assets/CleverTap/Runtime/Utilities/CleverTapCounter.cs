namespace CleverTapSDK.Utilities {
    internal class CleverTapCounter {
        private int _counter = 0;

        internal CleverTapCounter(int startFrom = 1) {
            _counter = startFrom;
        }

        internal int GetNextAndIncreaseCounter() =>
            _counter++;
    }
}
