% Test %
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%   Canna Prolog Initalization Library   %
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%(If ->  Then) :- If, !, Then.

findall(Template, Generator, List) :-
	save_instances(-Template, Generator),
	list_instances([], List).

Var ^ Goal :- Goal.
:- include(lists).
:- include(sort).
:- include(findall).

[X] :- consult(X).                          % consult shortcut



(If ->  Then) :- If, !, Then.
(If ->  Then; Else) :- !, (If  -> Then ; Else).
(A ; B) :- (A ; B).

','(Goal1, Goal2) :-			% Puzzle for beginners!
	Goal1,
	Goal2.
	



% op/3
op(Prec,Spec,[]) :- !.
op(Prec,Spec,[Op|T]) :- !,makeop(Prec,Spec,Op),op(Prec,Spec,T).
op(Prec,Spec,Op) :- makeop(Prec,Spec,Op).

compare(<, Term1, Term2) :-
    Term1 @< Term2,!.
compare(>, Term1, Term2) :-
    Term1 @> Term2,!.  
compare(=,Term1,Term2).

%IO Predicates
open(Src,Stream,Mode) :- open(Src,Stream,Mode,[]).
close(S_or_a) :- close(S_or_a,[]).
flush_output :- current_output(S),flush_output(S).

at_end_of_stream:- 
    current_input(S),
    stream_property(S, end_of_stream(E)),
    !, 
    (E = at; E = past).
    
at_end_of_stream(S_or_a) :- 
    ( atom(S_or-a) ->
        stream_property(S, alias(S_or_a))
            ;
        S = S_or_a
    ),
    stream_property(S, end_of_stream(E)),
    !, 
(E = at; E = past).



