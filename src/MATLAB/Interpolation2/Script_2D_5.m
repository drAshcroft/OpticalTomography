chosenTriad =bestTriangle;

slice1=allSlices{chosenTriad(1)};
slice2=allSlices{chosenTriad(2)};
slice3=allSlices{chosenTriad(3)};

for I=1:length(allSlices)
    allSlices{I}.AlreadyDone=false;
end
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%  first segment is just fixed to the x axis
[index12, index21, minVal1,bestSlice]=findCross(slice1,slice2,maxM);

allSlices{chosenTriad(1)}.Point1=[0,0,index12];
allSlices{chosenTriad(2)}.Point1=[0,0,index21];

[index13, index31, minVal2]=findCross(slice1,slice3,maxM);

L1=abs(index13-index12);
allSlices{chosenTriad(1)}.Point2=[L1,0,index13];
allSlices{chosenTriad(3)}.Point1=[L1,0,index31];

[index23, index32, minVal3]=findCross(slice2,slice3,maxM);

L2=abs(index23-index21);
L3=abs(index32-index31);

theta = acos(  (L1^2+L2^2-L3^2)/(2*L1*L2));
allSlices{chosenTriad(3)}.Point2=[L2*cos(theta), L2*sin(theta),index32];
allSlices{chosenTriad(2)}.Point2=[L2*cos(theta), L2*sin(theta),index23];

allSlices{chosenTriad(1)}.AlreadyDone=true;
allSlices{chosenTriad(2)}.AlreadyDone=true;
allSlices{chosenTriad(3)}.AlreadyDone=true;
goodHit=(minVal+minVal2+minVal3)/3*1.2;%%%%%%%%%%
slice1=allSlices{chosenTriad(1)};
slice2=allSlices{chosenTriad(2)};
slice3=allSlices{chosenTriad(3)};

canvas=PlotLines({slice1,slice2,slice3},bestSlice);
graymap(canvas);