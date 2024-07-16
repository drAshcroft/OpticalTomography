
%%Classify all the peaks in the sample
tMMM=MMM(:);
tCCC=CCC(:);
tLLL=LLL(:);
tIII=III(:);


for I=1:5
    maxM = max(tMMM)*.99;
    idx=find(tMMM> maxM);
    MM=tMMM( idx );
    LL=tLLL( idx) ;
    CC=tCCC(idx);
    
    %find the location of the max
    [m idx]=max(MM);
    PeakMax(I)=m;
    PeakColLoc(I)=CC(idx);
    maxLoc = CC(idx);
    PeakRowWidth(I)=LL(idx);
    
    idx=find(tCCC>maxLoc+10);
    idx2=find(tCCC<maxLoc-10);
    
    tMMM=vertcat(tMMM(idx), tMMM(idx2));
    tLLL=vertcat(tLLL(idx), tLLL(idx2));
    tCCC=vertcat(tCCC(idx), tCCC(idx2));
end

bestSlices=[];
bestIndex=[];
usefulSlices=[];
for I=1:length(allSlices)
    allSlices{I}.PeakAssignment=[];
    slice = allSlices{I};
    if isfield(slice, 'Peaks')
        cc=0;
        hasOne=false;
        for K=1:length(slice.Peaks)
            loc=slice.PeakLoc(:,1);
            for J=1:length(PeakMax)
                if (slice.Peaks(K)>PeakMax(J)*.90 && abs(loc(K)-PeakColLoc(J)) <10)
                    allSlices{I}.PeakAssignment(K)=J;
                    cc=cc+1;
                    if J==1 && slice.Peaks(K)>PeakMax(J)*.97 && hasOne==false
                        bestSlices=[bestSlices I];
                        bestIndex =[bestIndex K];
                        hasOne=true;
                    end
                end
            end
        end
        %now save those peaks that are the most useful for setting up
        %the triangle, best slices holds those that go through the main
        %peak
        if cc>1 && hasOne==false
            usefulSlices=[usefulSlices I];
        end
    end
end

% only pull those best slices that have multiple points of interest on them
doubledSlices =[];
doubledIndexs =[];
for I=1:length(bestSlices)
    slice =allSlices{bestSlices(I)}.PeakAssignment;
    idx=find(slice>0);
    idx2=find(slice==1);
    if length(idx)>1 && isempty(idx2)==false
        doubledSlices=[doubledSlices bestSlices(I)];
        doubledIndexs =[doubledIndexs bestIndex(I)];
    end
end


% now find the pairs
possiblePairs={};
pairIndexs={};
cc=1;
for I=1:length(doubledSlices)
    slice1=allSlices{ doubledSlices(I) }.PeakAssignment(:);
    slice1(slice1< 2 )=[];
    %find slices that form a triangle
    for J=I+1:length(doubledSlices)
        slice2=allSlices{ doubledSlices(J) }.PeakAssignment(:);
        slice2(slice2<2)=[];
        
        match=false;
        
        for K=1:length(slice1)
            if (isempty(find(slice2==slice1(K)))==false)
                match=true;
            else
                peak1=slice1(K);
                peak2=slice2(1);
            end
        end
        
        if match==false
            found=false;
            for K=1:length(pairIndexs)
                t=pairIndexs{K};
                if t(1)==peak1 && t(2)==peak2
                    found=true;
                end
            end
            
            if found==false
                possiblePairs{cc}= [doubledSlices(I) doubledSlices(J)];
                pairIndexs{cc}=[peak1 peak2];
                cc=cc+1;
            end
        end
    end
end

cc=1;
possibleTriangles={};
triangleSize=[];
triangleQuality=[];
for I=1:length(usefulSlices)
    slice=allSlices{usefulSlices(I)}.PeakAssignment;
    
    for J=1:length(pairIndexs)
        t=pairIndexs{J};
        if ( isempty( find(t(1)== slice)) ==false && isempty( find(t(2)== slice))==false )
            triad = [possiblePairs{J} usefulSlices(I)];
            
            slice1=allSlices{triad(1)};
            slice2=allSlices{triad(2)};
            slice3=allSlices{triad(3)};
            
            idx = find(slice1.PeakAssignment==1);
            idx2=find(slice1.PeakAssignment==t(1));
            sIndex1=1;
            if isempty( idx2) ==true
                idx2=find(slice1.PeakAssignment==t(2));
                sIndex1=2;
            end
            loc=slice1.PeakLoc(:,2);
            row1=abs( loc(idx)-loc(idx2));
            rowLoc=slice1.PeakLoc([idx idx2],2);
            
            A1_1=slice1.array(rowLoc(1),:);
            A1_2=slice1.array(rowLoc(2),:);
            
            idx = find(slice2.PeakAssignment==1);
            idx2=find(slice2.PeakAssignment==t(1));
            sIndex2=1;
            if isempty( idx2) ==true
                idx2=find(slice2.PeakAssignment==t(2));
                sIndex2=2;
            end
            loc=slice2.PeakLoc(:,2);
            row2=abs( loc(idx)-loc(idx2));
            rowLoc=slice2.PeakLoc([idx idx2],2);
            A2_1=slice2.array(rowLoc(1),:);
            A2_2=slice2.array(rowLoc(2),:);
            
            
            idx = find(slice3.PeakAssignment==t(1));
            idx2=find(slice3.PeakAssignment==t(2));
            loc=slice3.PeakLoc(:,2);
            row3=abs( loc(idx)-loc(idx2));
          
            rowLoc=slice3.PeakLoc([idx idx2],2);
            
            if (sIndex1==1)
                A3_1=slice3.array(rowLoc(1),:);
                A3_2=slice3.array(rowLoc(2),:);
            else
                A3_2=slice3.array(rowLoc(1),:);
                A3_1=slice3.array(rowLoc(2),:);
            end
            
            P=(row1+row2+row3);
            
            P2=P*(P-2*row1)*(P-2*row2)*(P-2*row3);
            if P2>0
                triangleSize(cc)=.25*(P2 )^.5;
                possibleTriangles{cc}=triad;
                triangleQuality(cc)=  sum((A1_1-A2_1).^2)^.5      +   sum((A1_2-A3_1).^2)^.5  +sum((A2_2-A3_2).^2)^.5;
                cc=cc+1;
            end
            
        end
    end
end
triangleQuality=triangleQuality';
possibleTriangles=possibleTriangles';
[tQuality idx]=sort(triangleQuality);

idx=idx(1:round(length(idx)/5));

possibleTriangles=possibleTriangles(idx);
triangleSize=triangleSize(idx);

[tSize idx]=sort(triangleSize);
possibleTriangles=possibleTriangles(idx);

bestTriangle=possibleTriangles{round(length(possibleTriangles)*.75)};


graymap(allSlices{bestTriangle(1)}.array);
graymap(allSlices{bestTriangle(2)}.array);
graymap(allSlices{bestTriangle(3)}.array);




