% Test %
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%   Canna Prolog Initalization Library   %
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


:- module('init',[consult/1,
ensure_loaded/1,
	(.) / 2,
	(^) / 2,
	(->) / 2,
	(;) / 2,
	(,) / 2,
	op/3,
	open/3,
	close/1,
	flush_output/0,
	at_end_of_stream/0,
	at_end_of_stream/1,
	write_term/2,
	write/1,
	writeq/2,
	write_canonical/1,
	write_canonical/2,
	findall/3,
	setof/3,
	bagof/3,
	member/2,
	append/3,
	select/3,
	nextto/3,
	delete/3,
	help/1]).
	
	
%(If ->  Then) :- If, !, Then.

%Loading
consult([]).
consult([File|Rest]) :- consult(File),consult(Rest).
consult(File) :- '$consult'(File).
[X] :- consult(X).                          % consult shortcut

ensure_loaded(File) :- '$consult'(File). 


Var ^ Goal :- Goal.
:- include(lists).
:- include(sort).
:- include(findall).




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


write_term(Term, Options):- 
current_output(S),
write_term(S, Term, Options). 
write(Term):- 
current_output(S),
write_term(S, Term, [numbervars(true)]). 
write_term(S, Term) :- 
write_term(S, Term, [numbervars(true)]). 
writeq(Term):- 
current_output(S),
write_term(S, Term, [quoted(true),numbervars(true)]). 
writeq(S,Term):- 
write_term(S, Term, [quoted(true),numbervars(true)]). 
write_canonical(Term) :- 
current_output(S),
write_term(S, Term, [quoted(true),ignore_ops(true)]). 
write_canonical(S,Term) :- 
write_term(S, Term, [quoted(true),ignore_ops(true)]).
 
help(PI) :- '$help'(PI,Help),showhelp(Help).
showhelp(help(Functor/Arity,Args,Desc)) :- print_template(Functor,Args),nl,print_desc(Desc),nl.

print_desc(Desc) :- member(longdesc(D),Desc),!,print(D).
print_desc(Desc) :- member(desc(D),Desc),!,print(D).
print_desc(Desc) :- print('No info available').

print_template(Functor,Args) :-
	print(Functor),
	print('('),
	print_args(Args),
	print(')').

print_args([]).
print_args([LastArg]) :- print_arg(LastArg).
print_args([Arg|Tail]) :- print_arg(Arg),print((,)),print_args(Tail).

print_arg('In'(Arg)) :- print(+),print(Arg).
print_arg('Out'(Arg)) :- print(-),print(Arg).
print_arg('Both'(Arg)) :- print(?),print(Arg).
print_arg(X) :- print(X).