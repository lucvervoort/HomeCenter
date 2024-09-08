using System;
using System.Collections.Generic;
using System.Text;
using Canna.Prolog.Runtime.Objects;

namespace Canna.Prolog.Runtime.Help
{
    [PrologPredicate(Name = "$help", Arity = 2)]
    public class dollarhelp_1 : BasePredicate
    {
        Term _pi, _ipi, _help, _ihelp;
        public dollarhelp_1(IPredicate continuation, IEngine engine, Term pi, Term help)
            : base(continuation, engine)
        {
            _ipi = pi;
            _ihelp = help;
        }

        public override PredicateResult Call()
        {
            _pi = _ipi.Dereference();
            _help = _ihelp.Dereference();

            if (!_pi.IsBound)
            {
                throw new InstantiationException(this);
            }
            PredicateIndicator pi = PredicateIndicator.FromTerm(_pi, this);

            Help.PredicatesPredicate help = HelpManager.Current.GetPredicateHelp(pi);

            Structure helpstr = HelpToStructure(help);
            if (!helpstr.Unify(_help, Engine.BoundedVariables, false))
            {
                return Fail();
            }

            return base.Call();
        }

        private Structure HelpToStructure(PredicatesPredicate help)
        {
            Structure pred = new Structure("help");
            pred.AddArg(new Structure("/", new Structure(help.PredicateIndicator.Functor),
                                            new Integer( help.PredicateIndicator.Arity)));
            PrologList list = new PrologList();
            foreach(Help.PredicatesPredicateArguments arg in help.Arguments)
            {
                list = list.Append(new PrologList(
                    new Structure(arg.Argument.Direction.ToString(), new Structure(arg.Argument.Name))));
            }
            pred.AddArg(list);
            list = new PrologList();
            if(help.Description !=null)
                list = list.Append(new PrologList(new Structure("desc", new Structure(help.Description))));
            if(help.LongDesc != null)
                list = list.Append(new PrologList(new Structure("longdesc", new Structure(help.LongDesc))));
            pred.AddArg(list);
            return pred;
        }

    }
}
