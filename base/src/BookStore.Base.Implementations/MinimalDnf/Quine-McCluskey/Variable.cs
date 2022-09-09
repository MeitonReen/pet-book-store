namespace BookStore.Base.Implementations.MinimalDnf.Quine_McCluskey
{
    /// <summary>
    ///   A model for the individual letters of a minterm.
    /// </summary>
    public class Variable
    {
        public Variable(char letter, bool notFlagSet)
        {
            Letter = letter;
            NotFlagSet = notFlagSet;
        }

        public char Letter { get; }
        private bool NotFlagSet { get; }

        public override string ToString()
            => (NotFlagSet ? "!" : "") + Letter;
    }
}