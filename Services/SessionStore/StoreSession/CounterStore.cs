namespace ConsolaBlazor.Services.SessionStore.StoreSession
{
    public class CounterStore
    {
        public int count { get; private set; } = 0;

        public void increment()
        {
            count++;
        }

        public void decrement()
        {
            count--;
        }
    }
}
