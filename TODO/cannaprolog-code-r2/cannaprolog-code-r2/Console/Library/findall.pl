%   findall(Template, Generator, List)
%   is a special case of bagof, where all free variables in the
%   generator are taken to be existentially quantified.  It is
%   described in Clocksin & Mellish on p152.  The code they give
%   has a bug (which the Dec-10 bagof and setof predicates share)
%   which this has not.

findall(Template, Generator, List) :-
	save_instances(-Template, Generator),
	list_instances([], List).



%   findall(Template, Generator, SoFar, List) :-
%	findall(Template, Generator, Solns),
%	append(Solns, SoFar, List).
%   But done more cheaply.

findall(Template, Generator, SoFar, List) :-
	save_instances(-Template, Generator),
	list_instances(SoFar, List).


%   set_of(Template, Generator, Set)
%   finds the Set of instances of the Template satisfying the Generator.
%   The set is in ascending order (see compare/3 for a definition of
%   this order) without duplicates, and is non-empty.  If there are
%   no solutions, set_of fails.  set_of may succeed more than one way,
%   binding free variables in the Generator to different values.  This
%   predicate is defined on p51 of the Dec-10 Prolog manual.

setof(Template, Filter, Set) :-
	bagof(Template, Filter, Bag),
	sort(Bag, Set).



%   bag_of(Template, Generator, Bag)
%   finds all the instances of the Template produced by the Generator,
%   and returns them in the Bag in they order in which they were found.
%   If the Generator contains free variables which are not bound in the
%   Template, it assumes that this is like any other Prolog question
%   and that you want bindings for those variables.  (You can tell it
%   not to bother by using existential quantifiers.)
%   bag_of records three things under the key '.':
%	the end-of-bag marker	       -
%	terms with no free variables   -Term
%	terms with free variables   Key-Term
%   The key '.' was chosen on the grounds that most people are unlikely
%   to realise that you can use it at all, another good key might be ''.
%   The original data base is restored after this call, so that set_of
%   and bag_of can be nested.  If the Generator smashes the data base
%   you are asking for trouble and will probably get it.
%   The second clause is basically just findall, which of course works in
%   the common case when there are no free variables.

bagof(Template, Generator, Bag) :-
	free_variables(Generator, Template, [], Vars),
	Vars \== [],
	!,
	Key =.. [.|Vars],
	functor(Key, ., N),
	save_instances(Key-Template, Generator),
	list_instances(Key, N, [], OmniumGatherum),
	keysort(OmniumGatherum, Gamut), !,
	concordant_subset(Gamut, Key, Answer),
	Bag = Answer.
bagof(Template, Generator, Bag) :-
	save_instances(-Template, Generator),
	list_instances([], Bag),
	Bag \== [].



%   save_instances(Template, Generator)
%   enumerates all provable instances of the Generator and records the
%   associated Template instances.  Neither argument ends up changed.

save_instances(Template, Generator) :-
	asserta( bag_of(-)),
	call(Generator),
	asserta( bag_of(Template)),
	fail.
save_instances(_, _).


%   list_instances(SoFar, Total)
%   pulls all the -Template instances out of the data base until it
%   hits the - marker, and puts them on the front of the accumulator
%   SoFar.  This routine is used by findall/3-4 and by bag_of when
%   the Generator has no free variables.

list_instances(SoFar, Total) :-
	retract(bag_of(Term)),
	!,		%   must not backtrack
	list_instances(Term, SoFar, Total).


list_instances(-, SoFar, Total) :- !,
	Total = SoFar.		%   = delayed in case Total was bound
list_instances(-Template, SoFar, Total) :-
	list_instances([Template|SoFar], Total).



%   list_instances(Key, NVars, BagIn, BagOut)
%   pulls all the Key-Template instances out of the data base until
%   it hits the - marker.  The Generator should not touch recordx(.,_,_).
%   Note that asserting something into the data base and pulling it out
%   again renames all the variables; to counteract this we use replace_
%   key_variables to put the old variables back.  Fortunately if we
%   bind X=Y, the newer variable will be bound to the older, and the
%   original key variables are guaranteed to be older than the new ones.
%   This replacement must be done @i<before> the keysort.

list_instances(Key, NVars, OldBag, NewBag) :-
	retract(bag_of(Term)),
	!,		%  must not backtrack!
	list_instances(Term, Key, NVars, OldBag, NewBag).


	list_instances(-, _, _, AnsBag, AnsBag) :- !.
	list_instances(NewKey-Term, Key, NVars, OldBag, NewBag) :-
		replace_key_variables(NVars, Key, NewKey), !,
		list_instances(Key, NVars, [NewKey-Term|OldBag], NewBag).



%   There is a bug in the compiled version of arg in Dec-10 Prolog,
%   hence the rather strange code.  Only two calls on arg are needed
%   in Dec-10 interpreted Prolog or C-Prolog.

replace_key_variables(0, _, _) :- !.
replace_key_variables(N, OldKey, NewKey) :-
	arg(N, NewKey, Arg),
	nonvar(Arg), !,
	M is N-1,
	replace_key_variables(M, OldKey, NewKey).
replace_key_variables(N, OldKey, NewKey) :-
	arg(N, OldKey, OldVar),
	arg(N, NewKey, OldVar),
	M is N-1,
	replace_key_variables(M, OldKey, NewKey).



%   concordant_subset([Key-Val list], Key, [Val list]).
%   takes a list of Key-Val pairs which has been keysorted to bring
%   all the identical keys together, and enumerates each different
%   Key and the corresponding lists of values.

concordant_subset([Key-Val|Rest], Clavis, Answer) :-
	concordant_subset(Rest, Key, List, More),
	concordant_subset(More, Key, [Val|List], Clavis, Answer).


%   concordant_subset(Rest, Key, List, More)
%   strips off all the Key-Val pairs from the from of Rest,
%   putting the Val elements into List, and returning the
%   left-over pairs, if any, as More.

concordant_subset([Key-Val|Rest], Clavis, [Val|List], More) :-
	Key == Clavis,
	!,
	concordant_subset(Rest, Clavis, List, More).
concordant_subset(More, _, [], More).


%   concordant_subset/5 tries the current subset, and if that
%   doesn't work if backs up and tries the next subset.  The
%   first clause is there to save a choice point when this is
%   the last possible subset.

concordant_subset([],   Key, Subset, Key, Subset) :- !.
concordant_subset(_,    Key, Subset, Key, Subset).
concordant_subset(More, _,   _,   Clavis, Answer) :-
	concordant_subset(More, Clavis, Answer).

%   In order to handle variables properly, we have to find all the 
%   universally quantified variables in the Generator.  All variables
%   as yet unbound are universally quantified, unless
%	a)  they occur in the template
%	b)  they are bound by X^P, setof, or bagof
%   free_variables(Generator, Template, OldList, NewList)
%   finds this set, using OldList as an accumulator.

free_variables(Term, Bound, VarList, [Term|VarList]) :-
	var(Term),
	term_is_free_of(Bound, Term),
	list_is_free_of(VarList, Term),
	!.
free_variables(Term, Bound, VarList, VarList) :-
	var(Term),
	!.
free_variables(Term, Bound, OldList, NewList) :-
	explicit_binding(Term, Bound, NewTerm, NewBound),
	!,
	free_variables(NewTerm, NewBound, OldList, NewList).
free_variables(Term, Bound, OldList, NewList) :-
	functor(Term, _, N),
	free_variables(N, Term, Bound, OldList, NewList).

free_variables(0, Term, Bound, VarList, VarList) :- !.
free_variables(N, Term, Bound, OldList, NewList) :-
	arg(N, Term, Argument),
	free_variables(Argument, Bound, OldList, MidList),
	M is N-1, !,
	free_variables(M, Term, Bound, MidList, NewList).

%   explicit_binding checks for goals known to existentially quantify
%   one or more variables.  In particular \+ is quite common.

explicit_binding(\+ Goal,	       Bound, fail,	Bound      ) :- !.
explicit_binding(not(Goal),	       Bound, fail,	Bound	   ) :- !.
explicit_binding(Var^Goal,	       Bound, Goal,	Bound+Var) :- !.
explicit_binding(setof(Var,Goal,Set),  Bound, Goal-Set, Bound+Var) :- !.
explicit_binding(bagof(Var,Goal,Bag),  Bound, Goal-Bag, Bound+Var) :- !.
explicit_binding(set_of(Var,Goal,Set), Bound, Goal-Set, Bound+Var) :- !.
explicit_binding(bag_of(Var,Goal,Bag), Bound, Goal-Bag, Bound+Var) :- !.


term_is_free_of(Term, Var) :-
	var(Term), !,
	Term \== Var.
term_is_free_of(Term, Var) :-
	functor(Term, _, N),
	term_is_free_of(N, Term, Var).

term_is_free_of(0, Term, Var) :- !.
term_is_free_of(N, Term, Var) :-
	arg(N, Term, Argument),
	term_is_free_of(Argument, Var),
	M is N-1, !,
	term_is_free_of(M, Term, Var).


list_is_free_of([Head|Tail], Var) :-
	Head \== Var,
	!,
	list_is_free_of(Tail, Var).
list_is_free_of([], _).



  
likes(bill, cider).
likes(dick, beer).
likes(harry, beer).
likes(jan, cider).
likes(tom, beer).
likes(tom, cider).

