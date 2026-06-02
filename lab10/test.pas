program TestParser; 

var
  a, b : integer;
  c real;   
  d : integer
  e, : integer;
  f : ;
  
  arr1 : array [1..10] of integer;
  arr2 : array 1..5] of integer;
  arr3 : array [1.5] of integer;
  arr4 : array [1..5 of integer;
  arr5 : array [1..5] integer;

  person : record
    age : integer;
    salary : real;
  end; 

  bad_record : record
    id : integer;

begin
  a := 10;
  
  b 5;
  
  if a > b then a := 0;

  arr1[1] := person.age + b;
  arr1[2 := 20;
  person. := 100;

  with person do
  begin
    age := 25;
  end;

  with person begin
    salary := 500.5;
  end;

  a := (b + 2;
  a := b + * 3;

end.