
for I=1:length(allSlices)
    allSlices{I}.AlreadyDone=false;
end

allSlices{chosenTriad(1)}.AlreadyDone=true;
allSlices{chosenTriad(2)}.AlreadyDone=true;
allSlices{chosenTriad(3)}.AlreadyDone=true;
goodHit=(minVal1+minVal2+minVal3)/3;
slice1=allSlices{chosenTriad(1)};
slice2=allSlices{chosenTriad(2)};
slice3=allSlices{chosenTriad(3)};

L=size(slice1.array,1);
[p1x p1y]=findPoint(slice1,1);
[p2x p2y]=findPoint(slice1,L);
[p3x p3y]=findPoint(slice2,L);
[cellCenter Radius]=FindCircle([p1x p1y],[p2x p2y],[p3x p3y])
slices={slice1,slice2,slice3};
Radius=Radius*1.01;

DrawSlices=chosenTriad(:)';
for I=1:length(allSlices)
    if (allSlices{I}.AlreadyDone==false)
        
        [idx1_T idxT_1 minVal1]= findCross(slice1,allSlices{I},maxM);
        [idx2_T idxT_2 minVal2]= findCross(slice2,allSlices{I},maxM);
        [idx3_T idxT_3 minVal3]= findCross(slice3,allSlices{I},maxM);
        
        indexS=[idx1_T idx2_T idx3_T];
        indexT=[idxT_1 idxT_2 idxT_3];
        
        [m idx]=sort([minVal1 minVal2 minVal3]);
        indexS=indexS(idx);
        indexT=indexT(idx);
        %         if I==13
        %            I
        %         end
       % if idx(1)==3 && idx(2)==2
            if m(1)<goodHit && m(2)<goodHit
                [x y]=findPoint(slices{idx(1)},indexS(1));
                P1=[x y indexT(1)];
                [x y]=findPoint(slices{idx(2)},indexS(2));
                P2=[x y indexT(2)];
                
                if (abs(P1(1)-P2(1))>3 && abs(P1(2)-P2(2))>3 && abs(P1(3)-P2(3))>3)
                    allSlices{I}.Point1=P1;
                    allSlices{I}.Point2=P2;
                    
                    [x y]=findPoint( allSlices{I},1);
                    R=((x-cellCenter(1))^2+(y-cellCenter(2))^2)^.5;
                    if (R>Radius)
                        I
                    else
                        allSlices{I}.AlreadyDone=true;
                        DrawSlices=[DrawSlices, I];
                    end
                    
                    
                end
            end
       % end
    end
end
canvas = PlotLines(allSlices(DrawSlices),bestSlice);
graymap(canvas);

goodHit=goodHit*1.05;

for I=1:length(allSlices)
    if (allSlices{I}.AlreadyDone==false)
        
        [idx1_T idxT_1 minVal1]= findCross(slice1,allSlices{I},maxM);
        [idx2_T idxT_2 minVal2]= findCross(slice2,allSlices{I},maxM);
        [idx3_T idxT_3 minVal3]= findCross(slice3,allSlices{I},maxM);
        
        indexS=[idx1_T idx2_T idx3_T];
        indexT=[idxT_1 idxT_2 idxT_3];
        
        [m idx]=sort([minVal1 minVal2 minVal3]);
        indexS=indexS(idx);
        indexT=indexT(idx);
        %         if I==13
        %            I
        %         end
       % if idx(1)==3 && idx(2)==2
            if m(1)<goodHit && m(2)<goodHit
                [x y]=findPoint(slices{idx(1)},indexS(1));
                P1=[x y indexT(1)];
                [x y]=findPoint(slices{idx(2)},indexS(2));
                P2=[x y indexT(2)];
                
                if (abs(P1(1)-P2(1))>3 && abs(P1(2)-P2(2))>3 && abs(P1(3)-P2(3))>3)
                    allSlices{I}.Point1=P1;
                    allSlices{I}.Point2=P2;
                    
                    [x y]=findPoint( allSlices{I},1);
                    R=((x-cellCenter(1))^2+(y-cellCenter(2))^2)^.5;
                    if (R>Radius)
                        I
                    else
                        allSlices{I}.AlreadyDone=true;
                        DrawSlices=[DrawSlices, I];
                    end
                    
                    
                end
            end
       % end
    end
end


