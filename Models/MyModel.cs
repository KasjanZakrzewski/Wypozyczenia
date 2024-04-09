namespace WypozyczeniaAPI.Models
{
    public class MyModel<T> // dwie listy zawierające obiekty klasy T
    {
        public IEnumerable<T> List1 { get; set; }
        public IEnumerable<T> List2 { get; set; }

    }
    public class MyModel<T,S> // dwie listy zawierające obiekty, odpowiednio klas T i S
    {
        public IEnumerable<T> List1 { get; set; }
        public IEnumerable<S> List2 { get; set; }

    }
    public class MyModel<T, S, R> // trzy listy zawierające obiekty, odpowiednio klas T, S i R
    {
        public IEnumerable<T> List1 { get; set; }
        public IEnumerable<S> List2 { get; set; }
        public IEnumerable<R> List3 { get; set; }
    }
    public class MyModel<T, S, R, Q, P> // pięć listy zawierające obiekty, odpowiednio klas T, S, R, Q, P
    {
        public IEnumerable<T> List1 { get; set; }
        public IEnumerable<S> List2 { get; set; }
        public IEnumerable<R> List3 { get; set; }
        public IEnumerable<Q> List4 { get; set; }
        public IEnumerable<P> List5 { get; set; }
    }
}
