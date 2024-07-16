function [d]=DistToLine(p1,p2,x0)


n=p2(1:2)-p1(1:2);
n=n./( sum( n.*n)^.5);
a=p1(1:2);
p=x0;
d=sum( ((a-p)-(sum((a-p).*n).*n)).^2)^.5;

end