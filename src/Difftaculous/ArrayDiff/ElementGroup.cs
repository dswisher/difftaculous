
using System;


namespace Difftaculous.ArrayDiff
{
    /// <summary>
    /// An operation that can be applied on an element group to transform one into the other
    /// </summary>
    public enum Operation
    {
        /// <summary>
        /// The two element groups are equal
        /// </summary>
        Equal,

        /// <summary>
        /// Insert the elements from B into A
        /// </summary>
        Insert,

        /// <summary>
        /// Delete the elements from A
        /// </summary>
        Delete

        // TODO - replace?
    };



    /// <summary>
    /// A group of array elements that have the same status (match, insert, update, delete)
    /// </summary>
    internal class ElementGroup
    {
        public static ElementGroup Delete(int start, int end)
        {
            return new ElementGroup
            {
                Operation = Operation.Delete,
                StartA = start,
                EndA = end
            };
        }


        public static ElementGroup Insert(int start, int end)
        {
            return new ElementGroup
            {
                Operation = Operation.Insert,
                StartB = start,
                EndB = end
            };
        }


        public static ElementGroup Equal(int startA, int endA, int startB, int endB)
        {
            return new ElementGroup
            {
                Operation = Operation.Equal,
                StartA = startA,
                EndA = endA,
                StartB = startB,
                EndB = endB
            };
        }


        private ElementGroup()
        {
        }


        public Operation Operation { get; private set; }

        public int StartA { get; private set; }
        public int EndA { get; private set; }

        public int StartB { get; private set; }
        public int EndB { get; private set; }


        public void Extend(int amount)
        {
            switch (Operation)
            {
                case Operation.Equal:
                    EndA += amount;
                    EndB += amount;
                    break;

                case Operation.Delete:
                    EndA += amount;
                    break;

                case Operation.Insert:
                    EndB += amount;
                    break;

                default:
                    throw new NotImplementedException("Extend() for operation " + Operation + " is not yet implemented.");
            }
        }



        public override string ToString()
        {
            switch (Operation)
            {
                case Operation.Equal:
                    if ((StartA == StartB) && (EndA == EndB))
                    {
                        return string.Format("E({0}..{1})", StartA, EndA);
                    }
                    return string.Format("E({0}..{1},{2}..{3})", StartA, EndA, StartB, EndB);

                case Operation.Delete:
                    return string.Format("D({0}..{1})", StartA, EndA);

                case Operation.Insert:
                    return string.Format("I({0}..{1})", StartB, EndB);

                default:
                    throw new NotImplementedException("ToString() for operation " + Operation + " is not yet implemented.");
            }
        }



        public override bool Equals(object obj)
        {
            ElementGroup other = obj as ElementGroup;

            if (other == null)
            {
                return false;
            }

            return ((Operation == other.Operation)
                && (StartA == other.StartA)
                && (EndA == other.EndA)
                && (StartB == other.StartB)
                && (EndB == other.EndB));
        }



        public override int GetHashCode()
        {
            return Operation.GetHashCode() ^ StartA.GetHashCode() ^ EndA.GetHashCode() ^ StartB.GetHashCode() ^ EndB.GetHashCode();
        }
    }
}
