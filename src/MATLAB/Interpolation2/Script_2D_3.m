
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
Pairs=cell(size(SR));
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













