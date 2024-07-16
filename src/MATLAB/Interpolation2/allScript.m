
%[allSlices, block] = LoadInterpolateImage2D(50,25);




maxM=0;
for I=1:length(allSlices)
    image = allSlices{I}.array;
    m=max(image(:));
    maxM = max( [maxM m]);
end

maxM=maxM*.97;
myfilter = fspecial('gaussian',[19 19], 10);
Peaks=cell([length(allSlices) 1]);
PeakLoc = cell(size(Peaks));
PeakLength = cell(size(Peaks));
figure;colormap gray;
cc=1;
for I=1:length(allSlices)
    image = allSlices{I}.array;
    im2=imfilter(image,myfilter);
    im2=im2-maxM/2;
    im2(im2<0)=0;
    im2=imregionalmax(im2);
    
    [row col] = find (im2>0 );
    %     clf;
    %     imagesc( image);
    %     hold all;
    if length(col)>10
        col=col(1:10);
        row=row(1:10);
    end
    maxes = zeros([1 size(col,1)]);
    for J=1:length(col)
        maxes(J) = image(row(J),col(J));
    end
    idx = find(maxes<maxM*.7);
    maxes(idx)=[];
    col(idx)=[];
    row(idx)=[];
    
    L=image(row,:);
    %  idx = find();
    L(L<maxM*.3)=0;
    L(L~=0)=1;
    L=sum(L,2);
    
    [maxes idx]=sort(maxes);
    L=L(idx);
    col=col(idx);
    row=row(idx);
    
    idx=find(L==0);
    maxes(idx)=[];
    L(idx)=[];
    col(idx)=[];
    row(idx)=[];
    
    if isempty(maxes)==0
        cc3=2;
        cc2=1;
        bad=[];
        Last=1;
        for J=2:length(maxes)
            R=abs(col(Last)-col(J))   +abs(row(Last)-row(J));
            if (R<50)
                bad(cc2)=J;
                cc2=cc2+1;
            else
                Last=J;
            end
            
        end
        if isempty(bad)==0
            maxes(bad)=[];
            col(bad)=[];
            row(bad)=[];
            L(bad)=[];
        end
        
        top=min([4 length(maxes)]);
        
        maxes=maxes(1:top);
        col=col(1:top);
        row= row(1:top);
        L = L(1:top);
        
        allSlices{I}.Peaks=maxes;
        allSlices{I}.PeakLoc=[col row];
        allSlices{I}.Width = L;
        
        for J=1:length(maxes)
            if L(J)>50
                MMM(cc)=maxes(J);
                LLL(cc)=L(J);
                CCC(cc)=col(J);
                III(cc)=I;
                cc=cc+1;
            end
        end
    end
end


scatter3(MMM,CCC,LLL);


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
        for K=1:length(slice.Peaks)
            loc=slice.PeakLoc(:,1);
            cc=0;
            for J=1:length(PeakMax)
                if (slice.Peaks(K)>PeakMax(J)*.85 && abs(loc(K)-PeakColLoc(J)) <10)
                    allSlices{I}.PeakAssignment(K)=J;
                    cc=cc+1;
                    if J==1 && slice.Peaks(K)>PeakMax(1)*.97
                        bestSlices=[bestSlices I];
                        bestIndex = [bestIndex K];
                    end
                end
            end
            
            %now save those peaks that are the most useful for setting up
            %the triangle
            if cc>1
                usefulSlices=[usefulSlices I];
            end
        end
    end
end

doubledSlices =[];
doubledIndexs =[];
for I=1:length(bestSlices)
    slice =allSlices{bestSlices(I)}.PeakAssignment;
    idx=find(slice>0);
    if length(idx)>1
        doubledSlices=[doubledSlices bestSlices(I)];
        doubledIndexs =[doubledIndexs bestIndex(I)];
    end
end

pairDistance=zeros([length(MM)*length(MM) 1]);
clear Pairs;
cc=1;
for I=1:length(bestSlices)
    slice1=allSlices{bestSlices(I)};
    mm1=slice1.Peaks(bestIndex(I));
    ll1=slice1.Width(bestIndex(I));
    loc=slice1.PeakLoc(:,1);
    cc1=loc(bestIndex(I));
    for J=I+1:length(bestSlices)
        slice2=allSlices{bestSlices(J)};
        mm2=slice2.Peaks(bestIndex(J));
        ll2=slice2.Width(bestIndex(J));
        loc=slice2.PeakLoc(:,1);
        cc2=loc(bestIndex(J));
        
        pairDistance(cc)=(( mm1 -mm2 )^2 + (ll1-ll2)^2 + (cc1-cc2)^2) ^.5;
        Pairs{cc}=[bestSlices(I) bestSlices(J)];
        cc=cc+1;
    end
end

[pairDistance2 idx]=sort(pairDistance);
Pairs2=Pairs(idx);

idx=find(pairDistance2<2);
pairDistance=pairDistance2(idx);
Pairs=Pairs2(idx);


%search through the other peaks on these slices to find a nice peak
%that is far from the max peak
for I=1:length(Pairs)
    pair = Pairs{I};
    slice1=allSlices(pair(1));
    slice2=allSlices(pair(2));
    
end



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
goodHit=(minVal1+minVal2+minVal3)/3*1.2;%%%%%%%%%%
slice1=allSlices{chosenTriad(1)};
slice2=allSlices{chosenTriad(2)};
slice3=allSlices{chosenTriad(3)};

canvas=PlotLines({slice1,slice2,slice3},bestSlice);
graymap(canvas);

L=size(slice1.array,2);
[x y]=findPoint(slice1,1);
X=x;
Y=y;
[x y]=findPoint(slice2,1);
X=X+x;
Y=Y+y;
[x y]=findPoint(slice3,1);
X=X+x;
Y=Y+y;

[x y]=findPoint(slice1,L);
X=X+x;
Y=Y+y;
[x y]=findPoint(slice2,L);
X=X+x;
Y=Y+y;
[x y]=findPoint(slice3,L);
X=X+x;
Y=Y+y;
X=X/6;
Y=Y/6;

slices={slice1,slice2,slice3};

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
            if ( (x-X)^2 + (y-Y)^2 )^.5 < L/2*1.3
                
                [x y]=findPoint( allSlices{I},L);
                if ( (x-X)^2 + (y-Y)^2 )^.5 < L/2*1.3
                    
                    DrawSlices=[DrawSlices, I];
                end
            end
        end
    end
end
canvas = PlotLines(allSlices(DrawSlices),bestSlice);
graymap(canvas);











