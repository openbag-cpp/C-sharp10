program TestLexer;
var
  a: integer;
  s: string;
begin
  // 
  a := 10; 

  /* крутой тест */
  a := a + 5;

  { еще более крутой тест }
  a := a * 2;

  (* 
      скобка-звездочка *)
  a := a - 1;

  s := 'Hello, world!'; 

  s := 'Эта строка не закрыта и должна вызвать ошибку;
  
  a := 100;
end.