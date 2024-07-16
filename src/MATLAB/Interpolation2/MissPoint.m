function [x y]=MissPoint(slice,index)

p1=slice.Point1;
p2=slice.Point2;

uX=(p2(1)-p1(1))/(p2(3)-p1(3));
uY=(p2(2)-p1(2))/(p2(3)-p1(3));

x=uX*(index-p1(3)) + p1(1) + uY*2;
y=uY*(index-p1(3)) + p1(2)-uX*2;

end