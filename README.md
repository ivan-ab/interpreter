# Интерпретатор

## Лабораторная работа

Грамматика:

````
*Program: Statements
Statement: ExpressionStatement | Assignment | If | While | Break
ExpressionStatement: Expression ';'
Assignment: Identifier '=' Expression ';'
If: 'if' '(' Expression ')' '{' Statements '}'
While: 'while' '(' Expression ')' '{' Statements '}'
Break: 'break' ';'
Statements: (Statements Statement)?
Expression: ...
Call: Call '(' Parameters? ')' | Primary
Primary: '(' Expression ')' | number | identifier
Parameters: Parameters ',' Expression | Expression
````

Помимо установленных по дефолту переменных true и false, надо добавить dump. dump можно вызывать как функцию, она печатает аргументы в консоль.
И еще null: с ним ничего нельзя делать, только сравнивать, у null как бы отдельный тип, ещё dump возвращает null.

Входная программа должна читаться из файла.
