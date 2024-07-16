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
        
        if m(1)<goodHit
            [x y]=findPoint(slices{idx(1)},indexS(1));
            P1=[x y indexT(1)];
            [x y]=findPoint(slices{idx(2)},indexS(2));
            
            P2=[x y indexT(2)];
            allSlices{I}.Point1=P1;
            allSlices{I}.Point2=P2;
            
            [x y]=findPoint( allSlices{I},L);
            if ( (x-X)^2 + (y-Y)^2 )^.5 < L/2*1.15
                
                [x y]=findPoint( allSlices{I},L);
                if ( (x-X)^2 + (y-Y)^2 )^.5 < L/2*1.15
                    DrawSlices=[DrawSlices, I];
                end
            end
        end
    end
end
canvas = PlotLines(allSlices(DrawSlices),bestSlice);
graymap(canvas);


