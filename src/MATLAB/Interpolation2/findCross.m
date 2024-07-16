function [index1, index2, minVal,maxLoc]=findCross(slice1,slice2,maxM)

L=size(slice1.array,1);

LL1=zeros([size(slice1.array,1) 1]);
LL2=zeros(size(LL1));

for I=1:L
    A1=slice1.array(I,:);
    LL1(I)=length(find(A1>maxM*.3));
    A1=slice2.array(I,:);
    LL2(I)=length(find(A1>maxM*.3));
end


M=zeros(L);
for I=1:L
    A1=slice1.array(I,:);
    
    if (LL1(I)>50)
        for J=1:L
            if (LL2(J)>50 && abs(LL1(I)-LL2(J))<15)
                M(I,J)=(sum((A1-slice2.array(J,:)).^2)/(L-1))^.5;
            else
                M(I,J)=100000000;
            end
        end
    else
        for J=1:L
            M(I,J)=100000000;
        end
    end
end

[m idx]=min(M);
[minVal idxI]=min(m);
idxJ=idx(idxI);

index1=idxJ;
index2=idxI;

[m maxLoc]=max(slice1.array(index1,:));
%figure;plot(slice1.array(index1,:));hold all;plot(slice2.array(index2,:));

end
