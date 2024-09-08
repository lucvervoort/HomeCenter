%	member(?Elem, ?List)
%	
%	True if Elem is a member of List

member(X, [X|_]).
member(X, [_|T]) :-
	member(X, T).

%	append(?List1, ?List2, ?List1AndList2)
%	
%	List1AndList2 is the concatination of List1 and List2

append([], L, L).
append([H|T], L, [H|R]) :-
	append(T, L, R).
	
%	select(?Elem, ?List1, ?List2)
%
%	Is true when List1, with Elem removed results in List2.

select(X, [X|Tail], Tail).
select(Elem, [Head|Tail], [Head|Rest]) :-
	select(Elem, Tail, Rest).


%	nextto(?X, ?Y, ?List)
%	
%	True of Y follows X in List.

nextto(X, Y, [X,Y|_]).
nextto(X, Y, [_|Zs]) :-
	nextto(X, Y, Zs).

%	delete(?List1, ?Elem, ?List2)
%
%	Is true when Lis1, with all occurences of Elem deleted results in
%	List2.

delete([], _, []) :- !.
delete([Elem|Tail], Elem, Result) :- !, 
	delete(Tail, Elem, Result).
delete([Head|Tail], Elem, [Head|Rest]) :-
	delete(Tail, Elem, Rest).

