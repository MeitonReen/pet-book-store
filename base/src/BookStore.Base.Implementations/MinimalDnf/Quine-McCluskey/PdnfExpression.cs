using BookStore.Base.Implementations.MinimalDnf.Quine_McCluskey.Minterms;

namespace BookStore.Base.Implementations.MinimalDnf.Quine_McCluskey
{
    public class PdnfExpression
    {
        /// <summary>
        ///   Parses a string into <see cref="Minterm"/> fragments.
        /// </summary>
        /// <param name="input">A string containing an expression.</param>
        /// <exception cref="FormatException">
        ///   Thrown in the Minterm constructor if a subexpression is invalid; uncaught.
        /// </exception>
        public PdnfExpression(string input)
        {
            var rawMinterms = input.Split('+');

            foreach (var raw in rawMinterms)
                Minterms.Add(new Minterm(raw));
        }

        private List<MintermBase> Minterms { get; set; }
            = new List<MintermBase>();

        /// <summary>
        ///   Simplifies the <see cref="Minterms"/> contents
        ///   using the Q-M algorithm and returns the expression.
        /// </summary>
        public async Task<PdnfExpression> Simplify()
        {
            var currentBatch = Minterms;
            var simplification = new List<MintermBase>();
            List<MintermBase> foundMatches = null;

            do
            {
                if (foundMatches != null) // if not first iteration
                    currentBatch = foundMatches.Distinct().ToList();

                foundMatches = new List<MintermBase>();
                foreach (var minterm in currentBatch.OrderBy(m => m.NumberOfOnes))
                {
                    var potentialMatches = currentBatch
                        .Where(m => m.NumberOfOnes == minterm.NumberOfOnes + 1)
                        .Where(m => m.MatchesDomain(minterm));

                    foreach (var otherMinterm in potentialMatches)
                    {
                        if (await minterm.TryCombineWith(otherMinterm, out MintermBase combination))
                        {
                            foundMatches.Add(combination);
                            minterm.IsPrime = false;
                            otherMinterm.IsPrime = false;
                        }
                    }
                }

                // Minterms that couldn't be combined are included in final result
                simplification.AddRange(currentBatch.Where(m => m.IsPrime).Distinct());
            } while (foundMatches.Any());

            Minterms = simplification;
            return this;
        }

        public override string ToString()
            => Minterms
                .Select(m => m.Raw())
                .Aggregate((current, next) => current + " + " + next);
    }
}