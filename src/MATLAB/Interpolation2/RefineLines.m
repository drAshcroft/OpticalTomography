for I=1:length(allSlices)
    allSlices{I}.AlreadyDone=false;
end
for I=1:length(DrawSlices)
    allSlices{DrawSlices(I)}.AlreadyDone=true;
end

DrawSlices2=DrawSlices4(:)';
 goodHit=goodHit*1.02;
for Z=1:10
   
chosen=randperm(length(DrawSlices4));

chosen =DrawSlices(chosen);
cc=1;
d=R;
while d>Radius/4 && d<Radius/8
    slice1=allSlices{chosen(cc)};
    d=DistToLine(slice1.Point1,slice1.Point2,cellCenter);
    cc=cc+1;
end
while d>Radius/8
    slice2=allSlices{chosen(cc)};
    d=DistToLine(slice2.Point1,slice2.Point2,cellCenter);
    cc=cc+1;
end


slices={slice1,slice2,slice3};


%DrawSlices2=DrawSlices(:)';
for I=1:length(allSlices)
    if (allSlices{I}.AlreadyDone==false)
        
        [idx1_T idxT_1 minVal1]= findCross(slice1,allSlices{I},maxM);
        [idx2_T idxT_2 minVal2]= findCross(slice2,allSlices{I},maxM);
       % [idx3_T idxT_3 minVal3]= findCross(slice3,allSlices{I},maxM);
        
        indexS=[idx1_T idx2_T];
        indexT=[idxT_1 idxT_2];
        
        [m idx]=sort([minVal1 minVal2 ]);
        indexS=indexS(idx);
        indexT=indexT(idx);

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
                if (R<Radius)
                    allSlices{I}.AlreadyDone=true;
                    DrawSlices2=[DrawSlices2, I];
                end
                
                
            end-
        end
    end
end
canvas = PlotLines(allSlices(DrawSlices2),bestSlice);
graymap(canvas);
end