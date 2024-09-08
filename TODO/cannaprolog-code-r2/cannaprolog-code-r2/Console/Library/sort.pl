%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%
%    quicksort(+Unsorted, ?Sorted)
% 
%   The quicksort algorithm.

quicksort([],[]).

quicksort([X|Tail], Sorted) :-
    split(X, Tail, Small, Big),
    quicksort(Small, SortedSmall),
    quicksort(Big, SortedBig),
    append(SortedSmall, [X|SortedBig], Sorted).

split(X, [],[],[]).
split(X, [Y|Tail], [Y|Small], Big) :-
    X > Y, !,
    split(X, Tail, Small, Big).
split(X, [Y|Tail], Small, [Y|Big]) :-
    split(X, Tail, Small, Big).

sort(_, _, [], []).
sort(_, _, [X], [X]).
sort(Key, Order, [X,Y|L], Sorted) :-
	halve(L, [Y|L], Front, Back),
	sort(Key, Order, [X|Front], F),
	sort(Key, Order, Back, B),
	merge(Key, Order, F, B, Sorted).



halve([_,_|Count], [H|T], [H|F], B) :- !,
	halve(Count, T, F, B).
halve(_, B, [], B).


merge(Key, Order, [H1|T1], [H2|T2], [Hm|Tm]) :- !,
	compare(Key, Order, H1, H2, R),
	(   R = (<), !, Hm = H1, merge(Key, Order, T1, [H2|T2], Tm)
	;   R = (>), !, Hm = H2, merge(Key, Order, [H1|T1], T2, Tm)
	;   R = (=), !, Hm = H1, merge(Key, Order, T1, T2, Tm)
	).
merge(_, _, [], L, L) :- !.
merge(_, _, L, [], L).


compare(Key, Order, X, Y, R) :-
	compare(Key, X, Y, R0),
	combine(Order, R0, R).

compare(0, X, Y, R) :- !,
	compare(R, X, Y).
compare(N, X, Y, R) :-
	arg(N, X, Xn),
	arg(N, Y, Yn),
	compare(R, Xn, Yn).


combine(<, R, R).
combine(=<, >, >) :- !.
combine(=<, _, <).
combine(>=, <, >) :- !.
combine(>=, _, <).
combine(>, <, >) :- !.
combine(>, >, <) :- !.
combine(>, =, =).


keysort(R, S) :-
	sort(1, =<, R, S).


msort(R, S) :-
	sort(0, =<, R, S).


sort(R, S) :-
	sort(0, <, R, S).


merge(A, B, M) :-
	merge(0, =<, A, B, M).
