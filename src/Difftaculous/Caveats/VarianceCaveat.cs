
using System;
using Difftaculous.Paths;


namespace Difftaculous.Caveats
{
    public class VarianceCaveat : ICaveat
    {
        private readonly double _allowance;


        public VarianceCaveat(DiffPath path, double allowance)
        {
            _allowance = allowance;
            Path = path;

            // TODO
        }


        public DiffPath Path { get; private set; }


        public bool IsAcceptable(string a, string b)
        {
            double ad;
            double bd;

            if (double.TryParse(a, out ad) && double.TryParse(b, out bd))
            {
                if (Math.Abs(ad - bd) < _allowance)
                {
                    return true;
                }
            }
            else
            {
                // TODO - should this emit a warning that they attempted to apply a numeric
                // caveat to something that isn't numeric?
                return false;
            }

            return false;
        }
    }
}
