:- module(test,[init/0]).

init:- 
  setof(X,likes(X,Y),Set),
  write(Set),
  fail;

 
likes(bill, cider).
likes(dick, beer).
likes(harry, beer).
likes(jan, cider).
likes(tom, beer).
likes(tom, cider).


invoke('system.datetime',now,[],Now),invoke(Now,addyears,[1],New),object_to_atom(New,A).
