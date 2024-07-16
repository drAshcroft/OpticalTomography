
%[allSlices, block] = LoadInterpolateImage2D(50,25);
clear allSlices;
for I=1:1200
  
   filename =  sprintf('C:\\Users\\bashc\\Documents\\MATLAB\\Interpolation2\\slices2\\image%04d.mat',I);
   A= open(filename);
  
   allSlices{I}=A.imagestruct;
end


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

Script_2D_3
